using Carter;
using Cinema.Features.Users;
using Cinema.Persistance;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Movies;

public sealed record UpdateMovieRequest(
    [FromRoute] Guid Id,
    string Title,
    string Description,
    int Duration,
    string Image)
    : IRequest<IResult>;

public sealed class UpdateMovieRequestValidator : AbstractValidator<UpdateMovieRequest>
{
    public UpdateMovieRequestValidator()
    {
        RuleFor(c => c.Title).NotEmpty();
        RuleFor(c => c.Description).NotEmpty();
        RuleFor(c => c.Duration).NotEmpty();
        RuleFor(c => c.Image).NotEmpty();
    }
}

public sealed class UpdateMovieRequestHandler(
    CinemaDbContext db,
    IValidator<UpdateMovieRequest> validator)
    : IRequestHandler<UpdateMovieRequest, IResult>
{
    public async Task<IResult> Handle(UpdateMovieRequest request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var movie = await db.Movies.SingleOrDefaultAsync(m => m.Id == request.Id, cancellationToken);

        if (movie is null)
        {
            return Results.NotFound();
        }

        movie.Title = request.Title;
        movie.Description = request.Description;
        movie.Duration = TimeSpan.FromMinutes(request.Duration);
        movie.Image = request.Image;

        db.Movies.Update(movie);

        await db.SaveChangesAsync(cancellationToken);

        return Results.Ok(movie.ToViewModel());
    }
}

public sealed class UpdateMovie : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPut("movies/{id:guid}", async (
            [FromBody] UpdateMovieRequest request,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) => 
                await sender.Send(request, cancellationToken))
            .WithOpenApi()
            .RequireAuthorization(ApplicationRoles.Admin)
            .Produces<MovieViewModel>(200)
            .Produces<IDictionary<string, string[]>>(400)
            .Produces(404);
    }
}
