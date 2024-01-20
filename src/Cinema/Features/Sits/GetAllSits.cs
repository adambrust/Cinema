using Carter;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Sits;

public sealed record GetAllSitsRequest : IRequest<IResult>;

public sealed class GetAllSitsRequestHandler(CinemaDbContext db)
    : IRequestHandler<GetAllSitsRequest, IResult>
{
    public async Task<IResult> Handle(GetAllSitsRequest request, CancellationToken cancellationToken)
    {
        var sits = await db.Sits.AsNoTracking().ToListAsync(cancellationToken);

        return Results.Ok(sits);
    }
}

public sealed class GetAllSits : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("sits", async (
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new GetAllSitsRequest(), cancellationToken))
            .WithOpenApi()
            .Produces<IEnumerable<Sit>>();
    }
}
