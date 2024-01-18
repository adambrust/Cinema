using Carter;
using Cinema.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed class GetAllHalls : ICarterModule
{
    private static async Task<IResult> Handle(
        [FromServices] CinemaDbContext db,
        CancellationToken cancellationToken)
    {
        var halls = await db.Halls.AsNoTracking().Select(h => h.Number).ToListAsync(cancellationToken);

        return Results.Ok(halls);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("halls", Handle);
    }
}
