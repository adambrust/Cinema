using Cinema.Features.Halls;
using Cinema.Features.Movies;
using Cinema.Features.Screenings;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Persistance;

public sealed class CinemaDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql("Data Source=perifen.ddns.net,5432;Integrated Security=false;User ID=postgres;Password=zaq1@WSX;");
    }

    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<Screening> Screenings { get; set; } = null!;
    public DbSet<Hall> Halls { get; set; } = null!;
}
