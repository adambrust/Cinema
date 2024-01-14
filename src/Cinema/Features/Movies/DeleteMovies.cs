using Carter;
using Cinema.Persistance;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

internal sealed class DeleteMovies : ICarterModule
{
    private static async Task<IResult> Handle(
        Guid id,
        CinemaDbContext db,
        CancellationToken cancellationToken)
    {
        var movie = await db.Movies.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (movie is null)
        {
            return Results.NotFound(id);
        }

        db.Movies.Remove(movie);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("api/movies/{id:guid}", Handle).WithName("DeleteMovie").WithOpenApi();
    }
}
