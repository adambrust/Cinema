using Carter;
using Cinema.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

internal sealed class GetAllMovies : ICarterModule
{
    private static async Task<IResult> Handle(CinemaDbContext db, CancellationToken cancellationToken)
    {
        var movies = await db.Movies.AsNoTracking().ToListAsync(cancellationToken);

        return Results.Ok(movies);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("api/movies", Handle).WithName("GetAllMovies").WithOpenApi();
    }
}
