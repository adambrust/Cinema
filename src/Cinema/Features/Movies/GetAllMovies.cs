﻿using Carter;
using Cinema.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

public sealed class GetAllMovies : ICarterModule
{
    private static async Task<IResult> Handle(
        [FromServices] CinemaDbContext db,
        CancellationToken cancellationToken)
    {
        var movies = await db.Movies.AsNoTracking().ToListAsync(cancellationToken);

        return Results.Ok(movies);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies", Handle);
    }
}
