using Carter;
using Cinema.Features.Common;
using Cinema.Features.Movies;
using Cinema.Features.Sits;
using Cinema.Features.Tickets;
using Cinema.Features.Users;
using Cinema.Persistance;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed record CreateTicketRequest(Guid MovieId, List<Guid> Sits) : IRequest<IResult>;

public sealed class CreateTicketRequestValidator : AbstractValidator<CreateTicketRequest>
{
    public CreateTicketRequestValidator(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();

        RuleFor(c => c.MovieId).NotEmpty();
        RuleFor(c => c.MovieId).IdExist<CreateTicketRequest, Movie>(serviceProvider);

        RuleFor(c => c.Sits).NotEmpty();
        RuleForEach(c => c.Sits).IdExist<CreateTicketRequest, Sit>(serviceProvider);
        RuleForEach(c => c.Sits).MustAsync(async (command, sitId, cancellationToken) =>
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();
            return !await db.Movies.AsNoTracking()
                .Where(m => m.Id == command.MovieId)
                .SelectMany(m => m.ReservedSits)
                .Select(s => s.Id)
                .ContainsAsync(sitId, cancellationToken);
        }).WithMessage("Sit is arledy reserved");
    }
}

public sealed class CreateTicketRequestHandler(
    CinemaDbContext db,
    IValidator<CreateTicketRequest> validator,
    UserManager<User> userManager,
    IHttpContextAccessor contextAccessor)
    : IRequestHandler<CreateTicketRequest, IResult>
{
    public async Task<IResult> Handle(CreateTicketRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var user = await userManager.GetUserAsync(contextAccessor.HttpContext!.User);

        var moive = await db.Movies.SingleAsync(s => s.Id == request.MovieId, cancellationToken);

        var sits = await db.Sits
            .Where(s => request.Sits.Contains(s.Id))
            .ToListAsync(cancellationToken);

        foreach (var sit in sits)
        {
            moive.ReservedSits.Add(sit);
        }

        var ticket = new Ticket
        {
            Id = Guid.NewGuid(),
            User = user!,
            Movie = moive,
            Sits = sits
        };

        await db.Tickets.AddAsync(ticket, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created("tickets", ticket.Id);
    }
}

public sealed class CreateTicket : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("tickets", async (
            [FromBody] CreateTicketRequest request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(request, cancellationToken))
            .WithOpenApi()
            .RequireAuthorization()
            .Produces(201)
            .Produces<IDictionary<string, string[]>>(400);
    }
}
