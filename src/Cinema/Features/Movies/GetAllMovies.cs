using Carter;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

public sealed record GetAllMoviesRequest : IRequest<IResult>;

public sealed class GetAllMoviesRequestHandler(CinemaDbContext db)
    : IRequestHandler<GetAllMoviesRequest, IResult>
{
    public async Task<IResult> Handle(GetAllMoviesRequest request, CancellationToken cancellationToken)
    {
        var movies = await db.Movies.AsNoTracking().ToListAsync(cancellationToken);

        return Results.Ok(movies.ToViewModel());
    }
}

public sealed class GetAllMovies : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("movies", async (
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new GetAllMoviesRequest(), cancellationToken))
            .WithOpenApi()
            .Produces<IEnumerable<MovieListViewModel>>();
    }
}
