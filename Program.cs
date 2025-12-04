using EpopsService.Data;
using Microsoft.EntityFrameworkCore;



var builder = WebApplication.CreateBuilder(args);

// Use Render $PORT environment variable
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";

// DB context (Supabase)
builder.Services.AddDbContext<EpopsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
);

var app = builder.Build();

// No migrations or EnsureCreated since table exists manually
// app.Services.CreateScope();  // <--- REMOVE ALL MIGRATION CODE

//------------------------------------------------------------
// Routes
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
        return Results.NotFound(new { message = "BookUid not found" });

    // If unregistered → update to registered
    if (record.Status.ToLower() == "unregistered")
    {
        record.Status = "registered";
        record.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }

    // Always return the final status
    return Results.Ok(new
    {
        bookUid = record.BookUid,
        status = record.Status,
        updatedAt = record.UpdatedAt
    });
});

// Bind runtime port correctly for Render
app.Run($"http://0.0.0.0:{port}");
