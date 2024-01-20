using Carter;
using Cinema.Features.Common;
using Cinema.Features.Sits;
using Cinema.Features.Tickets;
using Cinema.Features.Users;
using Cinema.Persistance;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed record CreateTicketRequest(Guid UserId, Guid ScreeningId, List<Guid> Sits) : IRequest<IResult>;

public sealed class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();

        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.UserId).IdExist<CreateTicketRequest, User>(serviceProvider);

        RuleFor(c => c.ScreeningId).NotEmpty();
        RuleFor(c => c.ScreeningId).IdExist<CreateTicketRequest, Screening>(serviceProvider);

        RuleFor(c => c.Sits).NotEmpty();
        RuleForEach(c => c.Sits).IdExist<CreateTicketRequest, Sit>(serviceProvider);
        RuleForEach(c => c.Sits).MustAsync(async (command, sitId, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
            return !await db.Screenings.AsNoTracking()
                .Where(s => s.Id == command.ScreeningId)
                .SelectMany(s => s.ReservedSits)
                .Select(s => s.Id)
                .ContainsAsync(sitId, cancellationToken);
        }).WithMessage("Sit is arledy reserved");
    }
}

public sealed class CreateTicketRequestHandler(CinemaDbContext db, IValidator<CreateTicketRequest> validator)
    : IRequestHandler<CreateTicketRequest, IResult>
{
    public async Task<IResult> Handle(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await db.Users.SingleAsync(u => u.Id == request.UserId, cancellationToken);

        var screening = await db.Screenings.SingleAsync(s => s.Id == request.ScreeningId, cancellationToken);

        var sits = await db.Sits
            .Where(s => request.Sits.Contains(s.Id))
            .ToListAsync(cancellationToken);

        foreach (var sit in sits)
        {
            screening.ReservedSits.Add(sit);
        }

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            User = user,
            Screening = screening,
            Sits = sits
        };

        await db.Tickets.AddAsync(ticket, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created("ticets", ticket.Id);
    }
}

public sealed class CreateTicket : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("ticets", async (
            [FromBody] CreateTicketRequest request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(request, cancellationToken))
            .RequireAuthorization()
            .Produces(201)
            .Produces<IDictionary<string, string[]>>(400);
    }
}
