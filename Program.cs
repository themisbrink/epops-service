using EpopsService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add EF Core + SQLite
builder.Services.AddDbContext<EpopsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Auto-create database & tables on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EpopsDbContext>();
    db.Database.EnsureCreated();
}

// Endpoint to insert/update data
app.MapPost("/update", async (BookData input, EpopsDbContext db) =>
{
    input.UpdatedAt = DateTime.UtcNow;
    db.BookData.Add(input);
    await db.SaveChangesAsync();
    return Results.Ok(new { status = "OK" });
});

// Example: get all rows
app.MapGet("/all", async (EpopsDbContext db) =>
    await db.BookData.ToListAsync());

app.Run();
