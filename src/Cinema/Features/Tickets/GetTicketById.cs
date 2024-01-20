using Carter;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Tickets;

public sealed record GetTicketByIdRequest(Guid Id) : IRequest<IResult>;

public sealed class GetTicketByIdRequestHandler(CinemaDbContext db)
    : IRequestHandler<GetTicketByIdRequest, IResult>
{
    public async Task<IResult> Handle(GetTicketByIdRequest request, CancellationToken cancellationToken)
    {
        var ticket = await db.Tickets
            .AsNoTracking()
            .Include(t => t.User)
            .Include(t => t.Screening)
            .Include(t => t.Sits)
            .SingleOrDefaultAsync(t => t.Id == request.Id, cancellationToken);

        if (ticket is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(ticket.ToViewModel());
    }
}

public class GetTicketById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("tickets/{id:guid}", async (
            Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new GetTicketByIdRequest(id), cancellationToken))
            .WithOpenApi()
            .Produces<TicketViewModel>(200)
            .Produces(404);
    }
}
