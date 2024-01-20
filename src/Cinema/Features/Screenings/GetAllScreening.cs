using Carter;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed record GetAllScreeningRequest : IRequest<IResult>;

public sealed class GetAllScreeningRequestHandler(CinemaDbContext db)
    : IRequestHandler<GetAllScreeningRequest, IResult>
{
    public async Task<IResult> Handle(GetAllScreeningRequest request, CancellationToken cancellationToken)
    {
        var screenings = await db.Screenings.AsNoTracking().ToListAsync(cancellationToken);

        return Results.Ok(screenings.ToViewModel());
    }
}

public sealed class GetAllScreening : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("screenings", async (
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new GetAllScreeningRequest(), cancellationToken))
            .WithOpenApi()
            .Produces<IEnumerable<ScreeningListViewModel>>(200);
    }
}
