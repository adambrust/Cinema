using Carter;
using Cinema.Persistance;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed class DeleteScreening : ICarterModule
{
    private static async Task<IResult> Handle(
        Guid id,
        [FromServices] CinemaDbContext db,
        CancellationToken cancellationToken)
    {
        var screening = await db.Screenings.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

        if (screening == null)
        {
            return Results.NotFound(id);
        }

        var tickets = db.Tickets.Where(t => t.Screening.Id == id);

        db.Tickets.RemoveRange(tickets);
        db.Screenings.Remove(screening);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("screenings/{id:guid}", Handle).RequireAuthorization("Admin");
    }
}
