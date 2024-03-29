﻿using Cinema.Features.Movies;
using Cinema.Features.Sits;
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
    public DbSet<Sit> Sits { get; set; } = null!;
    public DbSet<Ticket> Tickets { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Movie>().HasMany(s => s.ReservedSits).WithMany();
        builder.Entity<Ticket>().HasMany(t => t.Sits).WithMany();
    }
}
