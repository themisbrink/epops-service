using EpopsService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB context – Supabase Postgres
builder.Services.AddDbContext<EpopsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

// Ensure DB & tables exist (no migrations tooling needed)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EpopsDbContext>();
    db.Database.EnsureCreated();
}

// Simple endpoints
app.MapPost("/update", async (BookData input, EpopsDbContext db) =>
{
    input.UpdatedAt = DateTime.UtcNow;
    db.BookData.Add(input);
    await db.SaveChangesAsync();
    return Results.Ok(new { message = "OK" });
});

app.MapGet("/all", async (EpopsDbContext db) =>
    await db.BookData.OrderByDescending(b => b.UpdatedAt).ToListAsync());

app.Run();
