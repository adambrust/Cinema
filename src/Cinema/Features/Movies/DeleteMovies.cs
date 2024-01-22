using Carter;
using Cinema.Features.Users;
using Cinema.Persistance;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

public sealed record DeleteMovieRequest(Guid Id) : IRequest<IResult>;

public sealed class DeleteMoviesRequestHandler(CinemaDbContext db)
    : IRequestHandler<DeleteMovieRequest, IResult>
{
    public async Task<IResult> Handle(DeleteMovieRequest request, CancellationToken cancellationToken)
    {
        var movie = await db.Movies.SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (movie is null)
        {
            return Results.NotFound();
        }

        var tickets = db.Tickets.Where(t => t.Movie.Id == movie.Id);

        db.Tickets.RemoveRange(tickets);

        db.Movies.Remove(movie);

        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}

public sealed class DeleteMovies : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("movies/{id:guid}", async (
            Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new DeleteMovieRequest(id), cancellationToken))
            .WithOpenApi()
            .RequireAuthorization(ApplicationRoles.Admin)
            .Produces(200)
            .Produces(404);
    }
}
