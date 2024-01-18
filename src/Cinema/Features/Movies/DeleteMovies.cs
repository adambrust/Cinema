﻿using Carter;
using Cinema.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

public sealed class DeleteMovies : ICarterModule
{
    private static async Task<IResult> Handle(
        Guid id,
        [FromServices] CinemaDbContext db,
        CancellationToken cancellationToken)
    {
        var movie = await db.Movies.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (movie is null)
        {
            return Results.NotFound(id);
        }

        db.Movies.Remove(movie);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("movies/{id:guid}", Handle).RequireAuthorization("Admin");
    }
}
