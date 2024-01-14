using Carter;
using Cinema.Persistance;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

internal class Delete : ICarterModule
{
    private static async Task<IResult> Handle(
        Guid id,
        CinemaDbContext db,
        CancellationToken cancellationToken)
    {
        var screening = await db.Screenings.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (screening == null)
        {
            return Results.NotFound(id);
        }

        db.Screenings.Remove(screening);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created("api/screenings", screening.Id);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/screenings/{id:guid}", Handle).WithName("DeleteScreening").WithOpenApi();
    }
}
