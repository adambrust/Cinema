using Carter;
using Cinema.Features.Common;
using Cinema.Features.Movies;
using Cinema.Features.Users;
using Cinema.Persistance;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed record CreateScreeningRequest(Guid MovieId, DateTime Time) : IRequest<IResult>;

public sealed class CreateScreeningRequestValidator : AbstractValidator<CreateScreeningRequest>
{
    public CreateScreeningRequestValidator(IServiceProvider serviceProvider, IDateTimeProvider dateTime)
    {
        RuleFor(c => c.MovieId).NotEmpty();
        RuleFor(c => c.MovieId).IdExist<CreateScreeningRequest, Movie>(serviceProvider);

        RuleFor(c => c.Time).NotEmpty();
        RuleFor(c => c.Time).GreaterThan(dateTime.UtcNow);
    }
}

public sealed class CreateScreeningRequestHandler(
    CinemaDbContext db,
    IValidator<CreateScreeningRequest> validator)
    : IRequestHandler<CreateScreeningRequest, IResult>
{
    public async Task<IResult> Handle(CreateScreeningRequest request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var movie = await db.Movies.SingleAsync(m => m.Id == request.MovieId, cancellationToken);

        var screening = new Screening
        {
            Id = Guid.NewGuid(),
            Movie = movie,
            Time = request.Time,
            ReservedSits = []
        };

        await db.Screenings.AddAsync(screening, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created("screenings", screening.Id);
    }
}

public sealed class CreateScreening : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("screenings", async (
            [FromBody] CreateScreeningRequest request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(request, cancellationToken))
            .WithOpenApi()
            .RequireAuthorization(ApplicationRoles.Admin)
            .Produces(201)
            .Produces<IDictionary<string, string[]>>(400);
    }
}
