using Carter;
using Cinema.Persistance;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

public sealed class UpdateMovie : ICarterModule
{
    private sealed record Command(
        string Title,
        string Description,
        TimeSpan Duration,
        string Image);

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Title).NotEmpty();
            RuleFor(c => c.Description).NotEmpty();
            RuleFor(c => c.Duration).NotEmpty();
            RuleFor(c => c.Image).NotEmpty();
        }
    }

    private static async Task<IResult> Handle(
        Guid id,
        [FromBody] Command command,
        [FromServices] CinemaDbContext db,
        [FromServices] IValidator<Command> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(command);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var movie = await db.Movies.SingleOrDefaultAsync(m => m.Id == id, cancellationToken);

        if (movie is null)
        {
            return Results.NotFound(id);
        }

        movie.Title = command.Title;
        movie.Description = command.Description;
        movie.Duration = command.Duration;
        movie.Image = command.Image;

        db.Movies.Update(movie);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(movie);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("movies/{id:guid}", Handle).RequireAuthorization("Admin");
    }
}
