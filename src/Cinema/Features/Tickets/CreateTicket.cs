using Carter;
using Cinema.Features.Common;
using Cinema.Features.Tickets;
using Cinema.Features.Users;
using Cinema.Persistance;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed class CreateTicket : ICarterModule
{
    private sealed record Command(Guid UserId, Guid ScreeningId, HashSet<Sit> Sits);

    private sealed class Validator : AbstractValidator<Command>
    {
        public Validator(CinemaDbContext db)
        {
            RuleFor(c => c.UserId).NotEmpty();
            RuleFor(c => c.UserId).IdExist<Command, User>(db);

            RuleFor(c => c.ScreeningId).NotEmpty();
            RuleFor(c => c.ScreeningId).IdExist<Command, Screening>(db);

            RuleFor(c => c.Sits).NotEmpty();
            RuleForEach(c => c.Sits).MustAsync((command, sit, cancellationToken) =>
            {
                return db.Screenings.AsNoTracking()
                    .Where(s => s.Id == command.ScreeningId)
                    .SelectMany(s => s.Hall.Sits)
                    .ContainsAsync(sit, cancellationToken);
            }).WithMessage("Sit does not exist in this hall");
            RuleForEach(c => c.Sits).MustAsync(async (command, sit, cancellationToken) =>
            {
                return !await db.Screenings.AsNoTracking()
                    .Where(s => s.Id == command.ScreeningId)
                    .SelectMany(s => s.ReservedSits)
                    .ContainsAsync(sit, cancellationToken);
            }).WithMessage("Sit is arledy reserved");
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

        var user = await db.Users.AsNoTracking()
            .SingleAsync(u => u.Id == command.UserId, cancellationToken);

        var screening = await db.Screenings.AsNoTracking()
            .SingleAsync(s => s.Id == command.ScreeningId, cancellationToken);

        foreach (var sit in command.Sits)
        {
            screening.ReservedSits.Add(sit);
        }

        var ticket = new Ticket(Guid.NewGuid(), user, screening, command.Sits);

        await db.Tickets.AddAsync(ticket, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created();
    }

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("ticets", Handle).RequireAuthorization();
    }
}
