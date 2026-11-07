using Microsoft.EntityFrameworkCore;

public class SkorDb : DbContext
{
    public SkorDb(DbContextOptions<SkorDb> options) : base(options) { }

    public DbSet<Skor> Skorlar => Set<Skor>();
}