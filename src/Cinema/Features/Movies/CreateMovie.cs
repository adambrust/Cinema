using Carter;
using Cinema.Features.Users;
using Cinema.Persistance;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Features.Movies;

public sealed record CreateMovieRequest(
    string Title,
    string Description,
    int Duration,
    string Image)
    : IRequest<IResult>;

public sealed class CreateMovieRequestValidator : AbstractValidator<CreateMovieRequest>
{
    public CreateMovieRequestValidator()
    {
        RuleFor(c => c.Title).NotEmpty();
        RuleFor(c => c.Description).NotEmpty();
        RuleFor(c => c.Duration).NotEmpty();
        RuleFor(c => c.Image).NotEmpty();
    }
}

public sealed class CreateMovieRequestHandler(CinemaDbContext db, IValidator<CreateMovieRequest> validator)
    : IRequestHandler<CreateMovieRequest, IResult>
{
    public async Task<IResult> Handle(CreateMovieRequest request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var movie = new Movie
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Duration = TimeSpan.FromMinutes(request.Duration),
            Image = request.Image,
        };

        await db.Movies.AddAsync(movie, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Created("movies", movie.Id);
    }
}

public sealed class CreateMovie : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("movies", async (
            [FromBody] CreateMovieRequest request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(request, cancellationToken))
            .WithOpenApi()
            .RequireAuthorization(ApplicationRoles.Admin)
            .Produces(201)
            .Produces<IDictionary<string, string[]>>(400);
    }
}
