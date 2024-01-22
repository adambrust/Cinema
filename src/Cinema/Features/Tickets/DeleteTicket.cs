using Carter;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Tickets;

public sealed record DeleteTicketRequest(Guid Id) : IRequest<IResult>;

public sealed class DeleteTicketRequestHandler(CinemaDbContext db)
    : IRequestHandler<DeleteTicketRequest, IResult>
{
    public async Task<IResult> Handle(DeleteTicketRequest request, CancellationToken cancellationToken)
    {
        var ticket = await db.Tickets
            .Include(t => t.Movie)
            .Include(t => t.Sits)
            .SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (ticket is null)
        {
            return Results.NotFound();
        }

        var movie = await db.Movies
            .Include(m => m.ReservedSits)
            .SingleAsync(m => m.Id == ticket.Movie.Id, cancellationToken);

        movie.ReservedSits.RemoveAll(ticket.Sits.Contains);

        db.Tickets.Remove(ticket);

        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}

public sealed class DeleteTicket : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("tickets/{id:guid}", async (
            Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new DeleteTicketRequest(id), cancellationToken))
            .WithOpenApi()
            .RequireAuthorization()
            .Produces(204)
            .Produces(404);
    }
}
