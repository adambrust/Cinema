using Carter;
using Cinema.Features.Common;
using Cinema.Features.Movies;
using Cinema.Persistance;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed class CreateScreening : ICarterModule
{
    private sealed record Command(Guid MovieId, Guid HallId, DateTime Time);

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator(CinemaDbContext db, IDateTimeProvider dateTime)
        {
            RuleFor(c => c.MovieId).NotEmpty();
            RuleFor(c => c.MovieId).IdExist<Command, Movie>(db);

            RuleFor(c => c.HallId).NotEmpty();
            RuleFor(c => c.HallId).IdExist<Command, Hall>(db);

            RuleFor(c => c.Time).NotEmpty();
            RuleFor(c => c.Time).GreaterThan(dateTime.UtcNow);
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

        var movie = await db.Movies.AsNoTracking()
            .SingleAsync(m => m.Id == command.MovieId, cancellationToken);

        var hall = await db.Halls.AsNoTracking()
            .SingleAsync(h => h.Id == command.HallId, cancellationToken);

        var screening = new Screening(Guid.NewGuid(), movie, hall, command.Time, []);

        await db.Screenings.AddAsync(screening, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("screenings", Handle).RequireAuthorization("Admin");
    }
}
