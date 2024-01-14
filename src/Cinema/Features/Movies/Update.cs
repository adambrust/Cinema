using Carter;
using Cinema.Persistance;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

internal sealed class Update : ICarterModule
{
    private sealed record Command(string Title, string Description, TimeSpan Duration);

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(c => c.Title).NotEmpty();
            RuleFor(c => c.Description).NotEmpty();
            RuleFor(c => c.Duration).NotEmpty();
        }
    }

    private static async Task<IResult> Handle(
        Guid id,
        Command command,
        CinemaDbContext db,
        IValidator<Command> validator,
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

        movie.Update(command.Title, command.Description, command.Duration);

        db.Movies.Update(movie);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(movie);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/movies/{id:guid}", Handle).WithName("UpdateMovie").WithOpenApi();
    }
}
