using Carter;
using Cinema.Features.Users;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Tickets;

public sealed record GetAllTicketsRequest : IRequest<IResult>;

public sealed class GetAllTicketsRequestHandler(CinemaDbContext db)
    : IRequestHandler<GetAllTicketsRequest, IResult>
{
    public async Task<IResult> Handle(GetAllTicketsRequest request, CancellationToken cancellationToken)
    {
        var tickets = await db.Tickets.AsNoTracking().ToListAsync(cancellationToken);

        return Results.Ok(tickets.ToViewModel());
    }
}

public sealed class GetAllTickets : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("tickets", async (
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new GetAllTicketsRequest(), cancellationToken))
            .WithOpenApi()
            .RequireAuthorization(ApplicationRoles.Worker)
            .Produces<IEnumerable<TicketViewModel>>(200);
    }
}
