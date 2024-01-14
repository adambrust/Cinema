using Cinema.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

internal class GetAll
{
    private static async Task<IResult> Handle(CinemaDbContext db, CancellationToken cancellationToken)
    {
        var screenings = await db.Screenings.AsNoTracking().ToListAsync(cancellationToken);

        return Results.Ok(screenings);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/screenings", Handle).WithName("GetAllScreenings").WithOpenApi();
    }
}
