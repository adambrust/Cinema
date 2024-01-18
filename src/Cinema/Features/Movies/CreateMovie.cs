using Carter;
using Cinema.Persistance;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Features.Movies;

public sealed class CreateMovie : ICarterModule
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

        var movie = new Movie(
            Guid.NewGuid(),
            command.Title,
            command.Description,
            command.Duration,
            command.Image);

        await db.Movies.AddAsync(movie, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("movies", Handle).RequireAuthorization("Admin");
    }
}
