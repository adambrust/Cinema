using Carter;
using Cinema.Features.Common;
using Cinema.Persistance;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

internal sealed class Create : ICarterModule
{
    private sealed record Command(Guid MovieId, Guid HallId, DateTime Time);

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator(IDateTimeProvider dateTime)
        {
            RuleFor(c => c.MovieId).NotEmpty();
            RuleFor(c => c.HallId).NotEmpty();
            RuleFor(c => c.Time).NotEmpty();
            RuleFor(c => c.Time).GreaterThan(dateTime.UtcNow);
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

        var movie = await db.Movies.AsNoTracking()
            .SingleOrDefaultAsync(m => m.Id == command.MovieId, cancellationToken);

        if (movie is null)
        {
            return Results.NotFound(command.MovieId);
        }

        var hall = await db.Halls.AsNoTracking()
            .SingleOrDefaultAsync(h => h.Id == command.HallId, cancellationToken);

        if (hall is null)
        {
            return Results.NotFound(command.HallId);
        }

        var screening = Screening.Create(movie, hall, command.Time);

        await db.Screenings.AddAsync(screening, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created("api/screenings", screening.Id);
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("api/screenings", Handle).WithName("CreateScreening").WithOpenApi();
    }
}
