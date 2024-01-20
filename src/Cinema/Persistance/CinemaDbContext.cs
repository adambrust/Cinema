using Cinema.Features.Movies;
using Cinema.Features.Screenings;
using Cinema.Features.Tickets;
using Cinema.Features.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Persistance;

public sealed class CinemaDbContext(
    DbContextOptions<CinemaDbContext> options)
    : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<Screening> Screenings { get; set; } = null!;
    public DbSet<Hall> Halls { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Sit>().HasKey(s => new { s.Row, s.Column });
    }
}
