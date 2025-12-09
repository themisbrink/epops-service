using EpopsService.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using static EpopsService.Data.ChatModels;
//using Data;

var builder = WebApplication.CreateBuilder(args);

// Use Render $PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// CORS -- Allow calls from Shopify
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedFrontends", policy =>
    {
        policy.WithOrigins(
            "https://f8d652-65.myshopify.com",
            "https://www.epopspublishing.com",
            "https://epopspublishing.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// DB context (Supabase)
builder.Services.AddDbContext<EpopsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

// GaiaB Chat Client (NEW)
// The endpoint will be updated later when VPN is active
builder.Services.AddHttpClient("GaiaB", client =>
{
    // example — change later to internal VPN host:port
    client.BaseAddress = new Uri(builder.Configuration["GaiaB:Endpoint"] ?? "http://localhost:9999");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();
app.UseCors("AllowedFrontends");

//------------------------------------------------------------
// ENDPOINTS
//------------------------------------------------------------

// Add one test endpoint
app.MapGet("/", () => "Epops Service online ✔");

// GET all
app.MapGet("/all", async (EpopsDbContext db) =>
    await db.BookData.OrderByDescending(b => b.UpdatedAt).ToListAsync()
);

// BATCH CREATE
app.MapPost("/batch-create", async (BatchCreateRequest req, EpopsDbContext db) =>
{
    var rows = new List<BookData>();

    for (int i = 1; i <= req.PrintQuantity; i++)
    {
        var uid = $"{req.SourceItemId}x{req.ISBN}x{i}x{req.PrintQuantity}";

        rows.Add(new BookData
        {
            BookUid = uid,
            ISBN = req.ISBN,
            SourceItemId = req.SourceItemId,
            NumberInJob = i.ToString(),
            PrintQuantity = req.PrintQuantity.ToString(),
            Status = "Unregistered",
            UpdatedAt = DateTime.UtcNow
        });
    }

    db.BookData.AddRange(rows);
    await db.SaveChangesAsync();

    return new { Created = rows.Count, Sample = rows.Take(3) };
});

// register Book
// POST https://epops-service.onrender.com/register?bookUid=BOOK-2001x9786180000001x3x5
app.MapPost("/register", async (string bookUid, EpopsDbContext db) =>
{
    var record = await db.BookData.FirstOrDefaultAsync(b => b.BookUid == bookUid);

    if (record == null)
    {
        return Results.NotFound(new
        {
            bookUid,
            message = "BookiId not valid"
        });
    }

    var status = record.Status?.ToLowerInvariant() ?? "";

    if (status == "registered")
    {
        return Results.Ok(new
        {
            bookUid = record.BookUid,
            status = record.Status,
            updatedAt = record.UpdatedAt,
            message = "Already Registered"
        });
    }

    if (status == "unregistered")
    {
        record.Status = "registered";
        record.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            bookUid = record.BookUid,
            status = record.Status,
            updatedAt = record.UpdatedAt,
            message = "registration success"
        });
    }

    // Any other unexpected status → treat as not valid
    return Results.Ok(new
    {
        bookUid = record.BookUid,
        status = record.Status,
        updatedAt = record.UpdatedAt,
        message = "BookiId not valid"
    });
});

// chat message
app.MapPost("/chat", async (GaiaBChatRequest req, IHttpClientFactory http) =>
{
    if (string.IsNullOrWhiteSpace(req.Question))
        return Results.BadRequest(new { error = "Question is required" });

    var GaiaBClient = http.CreateClient("GaiaB");

    var payload = new
    {
        api_key = Environment.GetEnvironmentVariable("GAIAB_API_KEY"),
        app_name = Environment.GetEnvironmentVariable("GAIAB_APP_NAME"),
        app_logic_descr = (string?)null, // not used
        app_params = new
        {
            current_question = req.Question,
            history = req.History ?? new List<historyItem>()
        }
    };

    try
    {
        var response = await GaiaBClient.PostAsJsonAsync("/api/gaiab/apps/sync/", payload);

        var result = await response.Content.ReadFromJsonAsync<GaiaBSyncResponse>();

        if (result?.app_results?.response != null)
        {
            return Results.Ok(new
            {
                reply = result.app_results.response,
                job = result.job_id
            });
        }

        return Results.Ok(new
        {
            reply = "No response returned",
            raw = result
        });
    }
    catch (Exception ex)
    {
        return Results.Ok(new { reply = "⚠ GaiaB unreachable", error = ex.Message });
    }
});



// Bind runtime port correctly for Render
app.Run($"http://0.0.0.0:{port}");

record ChatRequest(string sessionId, string message);
