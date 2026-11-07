using IlkApi;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SkorDb>(options =>
    options.UseSqlite("Data Source=skorlar.db"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SkorDb>();
    db.Database.EnsureCreated();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

static string? Dogrula(Skor skor)
{
    if (string.IsNullOrWhiteSpace(skor.Oyuncu))
        return "Oyuncu adı boş olamaz.";

    if (skor.Oyuncu.Length > 30)
        return "Oyuncu adı en fazla 30 karakter olabilir.";

    if (skor.Puan < 0)
        return "Puan negatif olamaz.";

    return null;
}

static int KullaniciIdAl(ClaimsPrincipal kullanici) =>
int.Parse(kullanici.FindFirstValue(ClaimTypes.NameIdentifier)!);

app.MapGet("/merhaba", () => "Merhaba dünya, ben geri döndüm!");

app.MapGet("/skorlar", async (SkorDb db) =>
    await db.Skorlar.ToListAsync());

app.MapGet("/skorlar/{id}", async (int id, SkorDb db) =>
{
    var skor = await db.Skorlar.FindAsync(id);
    return skor is null ? Results.NotFound() : Results.Ok(skor);
});

app.MapGet("/skorlar/en-yuksek", async (SkorDb db) =>
{
    var enYuksek = await db.Skorlar
        .OrderByDescending(s => s.Puan)
        .FirstOrDefaultAsync();
    return enYuksek is null ? Results.NotFound() : Results.Ok(enYuksek);
});

app.MapPost("/skorlar", async (Skor yeniSkor, SkorDb db, ClaimsPrincipal kullanici) =>
{
    var hata = Dogrula(yeniSkor);
    if (hata is not null) return Results.BadRequest(hata);

    var skor = new Skor(0, yeniSkor.Oyuncu.Trim(), yeniSkor.Puan, KullaniciIdAl(kullanici));
    db.Skorlar.Add(skor);
    await db.SaveChangesAsync();
    return Results.Created($"/skorlar/{skor.Id}", skor);
}).RequireAuthorization();

app.MapDelete("/skorlar/{id}", async (int id, SkorDb db, ClaimsPrincipal kullanici) =>
{
    var skor = await db.Skorlar.FindAsync(id);
    if (skor is null) return Results.NotFound();
    if (skor.KullaniciId != KullaniciIdAl(kullanici)) return Results.Forbid();

    db.Skorlar.Remove(skor);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.MapPut("/skorlar/{id}", async (int id, Skor guncelSkor, SkorDb db, ClaimsPrincipal kullanici) =>
{
    var hata = Dogrula(guncelSkor);
    if (hata is not null) return Results.BadRequest(hata);

    var skor = await db.Skorlar.FindAsync(id);
    if (skor is null) return Results.NotFound();
    if (skor.KullaniciId != KullaniciIdAl(kullanici)) return Results.Forbid();

    db.Entry(skor).CurrentValues.SetValues(new Skor(id, guncelSkor.Oyuncu.Trim(), guncelSkor.Puan, skor.KullaniciId));
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();

app.MapPost("/kayit", async (KayitIstegi istek, SkorDb db) =>
{
    if (string.IsNullOrWhiteSpace(istek.KullaniciAdi) || istek.KullaniciAdi.Length > 30)
        return Results.BadRequest("Kullanıcı adı boş olamaz ve en fazla 30 karakter olabilir.");

    if (string.IsNullOrEmpty(istek.Sifre) || istek.Sifre.Length < 6)
        return Results.BadRequest("Şifre en az 6 karakter olmalı.");

    var kullaniciAdi = istek.KullaniciAdi.Trim();

    var varMi = await db.Kullanicilar.AnyAsync(k => k.KullaniciAdi == kullaniciAdi);
    if (varMi)
        return Results.Conflict("Bu kullanıcı adı zaten alınmış.");

    var kullanici = new Kullanici { KullaniciAdi = kullaniciAdi };
    kullanici.SifreHash = new PasswordHasher<Kullanici>().HashPassword(kullanici, istek.Sifre);

    db.Kullanicilar.Add(kullanici);
    await db.SaveChangesAsync();

    return Results.Created($"/kullanicilar/{kullanici.Id}", new { kullanici.Id, kullanici.KullaniciAdi });
});

app.MapPost("/giris", async (GirisIstegi istek, SkorDb db, IConfiguration config) =>
{
    if (string.IsNullOrWhiteSpace(istek.KullaniciAdi) || string.IsNullOrEmpty(istek.Sifre))
        return Results.Unauthorized();

    var kullaniciAdi = istek.KullaniciAdi.Trim();
    var kullanici = await db.Kullanicilar.FirstOrDefaultAsync(k => k.KullaniciAdi == kullaniciAdi);
    if (kullanici is null) return Results.Unauthorized();

    var sonuc = new PasswordHasher<Kullanici>().VerifyHashedPassword(kullanici, kullanici.SifreHash, istek.Sifre);
    if (sonuc == PasswordVerificationResult.Failed) return Results.Unauthorized();

    var anahtar = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
    var imza = new SigningCredentials(anahtar, SecurityAlgorithms.HmacSha256);

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
        new Claim(ClaimTypes.Name, kullanici.KullaniciAdi)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: imza);

    return Results.Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
});

app.MapGet("/ben", (ClaimsPrincipal kullanici) => new
{
    Ad = kullanici.Identity?.Name,
    Id = kullanici.FindFirstValue(ClaimTypes.NameIdentifier)
}).RequireAuthorization();

app.Run();

public record Skor(int Id, string Oyuncu, int Puan, int KullaniciId = 0);
public record KayitIstegi(string KullaniciAdi, string Sifre);
public record GirisIstegi(string KullaniciAdi, string Sifre);