using Carter;
using Cinema.Features.Users;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Tickets;

public sealed record GetAllTicketsRequest : IRequest<IResult>;

public sealed class GetAllTicketsRequestHandler(
    CinemaDbContext db,
    UserManager<User> userManager,
    IHttpContextAccessor contextAccessor)
    : IRequestHandler<GetAllTicketsRequest, IResult>
{
    public async Task<IResult> Handle(GetAllTicketsRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.GetUserAsync(contextAccessor.HttpContext!.User);

        var isWorker = await userManager.IsInRoleAsync(user!, ApplicationRoles.Admin);

        var tickets = db.Tickets.AsNoTracking();

        if (!isWorker)
        {
            tickets = tickets.Where(t => t.User.Id == user!.Id);
        }

        var results = await tickets
            .Include(t => t.User)
            .Include(t => t.Movie)
            .Include(t => t.Sits)
            .ToListAsync(cancellationToken);

        return Results.Ok(results.ToViewModel());
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
            .RequireAuthorization()
            .Produces<IEnumerable<TicketViewModel>>(200);
    }
}
