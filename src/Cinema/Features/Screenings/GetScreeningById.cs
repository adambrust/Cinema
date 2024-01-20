using Carter;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed record GetScreeningByIdRequest(Guid Id) : IRequest<IResult>;

public sealed class GetScreeningByIdRequestHandler(CinemaDbContext db)
    : IRequestHandler<GetScreeningByIdRequest, IResult>
{
    public async Task<IResult> Handle(GetScreeningByIdRequest request, CancellationToken cancellationToken)
    {
        var screening = await db.Screenings
            .AsNoTracking()
            .Include(s => s.Movie)
            .Include(s => s.ReservedSits)
            .SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (screening is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(screening.ToViewModel());
    }
}

public sealed class GetScreeningById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("screenings/{id:guid}", async (
            Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new GetScreeningByIdRequest(id), cancellationToken))
            .WithOpenApi()
            .Produces<ScreeningViewModel>(200)
            .Produces(404);
    }
}
