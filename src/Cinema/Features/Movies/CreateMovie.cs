using Carter;
using Cinema.Persistance;
using FluentValidation;

namespace Cinema.Features.Movies;

internal sealed class CreateMovie : ICarterModule
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

        var movie = Movie.Create(command.Title, command.Description, command.Duration);

        await db.Movies.AddAsync(movie, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created("api/movies", movie.Id);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/movies", Handle).WithName("CreateMovie").WithOpenApi();
    }
}
