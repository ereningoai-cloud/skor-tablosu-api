using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SkorDb>(options =>
options.UseSqlite("Data Source=skorlar.db"));

var app = builder.Build();

using ( var scope = app.Services.CreateScope()) 
{
    var db = scope.ServiceProvider.GetRequiredService<SkorDb>();
    db.Database.EnsureCreated();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/merhaba", () => "Merhaba dünya, ben geri döndüm!");

app.MapGet("/skorlar", async(SkorDb db) =>
await db.Skorlar.ToListAsync());

app.MapGet("/skorlar/{id}",async(int id, SkorDb db) =>
{
    var skor = await db.Skorlar.FindAsync(id);
    return skor is null ? Results.NotFound() : Results.Ok(skor);
});

app.MapGet("/skorlar/en-yuksek", async (SkorDb db) =>
{
    var enYuksek = await db.Skorlar
    .OrderByDescending(s=>s.Puan)
    .FirstOrDefaultAsync();
    return enYuksek is null ? Results.NotFound() : Results.Ok(enYuksek);
});

app.MapPost("/skorlar", async (Skor yeniSkor, SkorDb db) =>
{
    var skor = new Skor(0, yeniSkor.Oyuncu, yeniSkor.Puan);
    db.Skorlar.Add(skor);
    await db.SaveChangesAsync();
    return Results.Created($"/skorlar/{skor.Id}", skor);
});

app.Run();

public record Skor(int Id, string Oyuncu, int Puan);