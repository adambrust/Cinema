using Carter;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

public sealed record GetMovieByIdRequest(Guid Id) : IRequest<IResult>;

public sealed class GetMovieByIdRequestHandler(CinemaDbContext db)
    : IRequestHandler<GetMovieByIdRequest, IResult>
{
    public async Task<IResult> Handle(GetMovieByIdRequest request, CancellationToken cancellationToken)
    {
        var movie = await db.Movies
            .AsNoTracking()
            .Include(m => m.ReservedSits)
            .SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (movie is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(movie.ToViewModel());
    }
}

public class GetMovieById : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies/{id:guid}", async (
            Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new GetMovieByIdRequest(id), cancellationToken))
            .WithOpenApi()
            .Produces<MovieViewModel>(200)
            .Produces(404);
    }
}
