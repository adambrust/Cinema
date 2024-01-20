using Carter;
using Cinema.Features.Users;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed record GetAllHallsRequest : IRequest<IResult>;

public sealed class GetAllHallsRequestHandler(CinemaDbContext db)
    : IRequestHandler<GetAllHallsRequest, IResult>
{
    public async Task<IResult> Handle(GetAllHallsRequest request, CancellationToken cancellationToken)
    {
        var halls = await db.Halls.AsNoTracking().ToListAsync(cancellationToken);

        return Results.Ok(halls.ToViewModel());
    }
}

public sealed class GetAllHalls : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("halls", async (
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new GetAllHallsRequest(), cancellationToken))
            .WithOpenApi()
            .RequireAuthorization(ApplicationRoles.Admin)
            .Produces<IEnumerable<HallViewModel>>(200);
    }
}
