using EpopsService.Data;
using Microsoft.EntityFrameworkCore;
using EpopsService.Diagnostics;


//await DbTester.TestConnection();
//return;

var builder = WebApplication.CreateBuilder(args);

// DB context – Supabase Postgres
builder.Services.AddDbContext<EpopsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Ensure DB & tables exist (no migrations tooling needed)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EpopsDbContext>();
    db.Database.Migrate();
}

// Simple endpoints
app.MapPost("/update", async (BookData input, EpopsDbContext db) =>
{
    input.UpdatedAt = DateTime.UtcNow;
    db.BookData.Add(input);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "OK" });
});


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
            Status = "Created",
            UpdatedAt = DateTime.UtcNow
        });
    }

    db.BookData.AddRange(rows);
    await db.SaveChangesAsync();

    return Results.Ok(new
    {
        Created = rows.Count,
        Sample = rows.Take(3).ToList()  // returns 3 sample entries
    });
});


app.MapGet("/all", async (EpopsDbContext db) =>
    await db.BookData.OrderByDescending(b => b.UpdatedAt).ToListAsync());

app.Run();
