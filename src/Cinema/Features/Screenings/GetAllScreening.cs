using Carter;
using Cinema.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed class GetAllScreening : ICarterModule
{
    private static async Task<IResult> Handle(
        [FromServices] CinemaDbContext db,
        CancellationToken cancellationToken)
    {
        var screenings = await db.Screenings.AsNoTracking().ToListAsync(cancellationToken);

        return Results.Ok(screenings);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("screenings", Handle);
    }
}
