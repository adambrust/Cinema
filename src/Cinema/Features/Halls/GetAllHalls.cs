﻿using Carter;
using Cinema.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Halls;

internal sealed class GetAllHalls : ICarterModule
{
    private static async Task<IResult> Handle(CinemaDbContext db, CancellationToken cancellationToken)
    {
        var halls = await db.Halls.AsNoTracking().Select(h => h.Number).ToListAsync(cancellationToken);

        return Results.Ok(halls);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/halls", Handle).WithName("GetAllHalls").WithOpenApi();
    }
}