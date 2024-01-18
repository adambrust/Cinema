using Carter;
using Cinema.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Tickets;

public sealed class DeleteTicket : ICarterModule
{
    private static async Task<IResult> Handle(
        Guid id,
        [FromServices] CinemaDbContext db,
        CancellationToken cancellationToken)
    {
        var ticket = await db.Tickets.SingleOrDefaultAsync(t => t.Id == id, cancellationToken);

        if (ticket is null)
        {
            return Results.NotFound(id);
        }

        var screening = await db.Screenings.SingleAsync(s => s.Id == ticket.Screening.Id, cancellationToken);

        screening.ReservedSits.RemoveWhere(ticket.Sits.Contains);

        db.Tickets.Remove(ticket);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("ticets/{id:guid}", Handle).RequireAuthorization();
    }
}
