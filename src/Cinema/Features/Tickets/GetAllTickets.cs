using Carter;
using Cinema.Persistance;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Tickets;

public sealed class GetAllTickets : ICarterModule
{
    private sealed record Command(Guid UserId, Guid ScreeningId);

    private static async Task<IResult> Handle(
        [FromBody] Command command,
        [FromServices] CinemaDbContext db,
        CancellationToken cancellationToken)
    {
        var tickets = db.Tickets.AsNoTracking();

        if (command.UserId != Guid.Empty)
        {
            tickets = tickets.Where(t => t.User.Id == command.UserId);
        }

        if (command.ScreeningId != Guid.Empty)
        {
            tickets = tickets.Where(t => t.Screening.Id == command.ScreeningId);
        }

        return Results.Ok(await tickets.ToListAsync(cancellationToken));
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("tickets", Handle).RequireAuthorization("Worker");
    }
}
