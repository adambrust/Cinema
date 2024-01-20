using Carter;
using Cinema.Features.Users;
using Cinema.Persistance;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Features.Screenings;

public sealed record DeleteScreeningRequest(Guid Id) : IRequest<IResult>;

public sealed class DeleteScreeningRequestHandler(CinemaDbContext db)
    : IRequestHandler<DeleteScreeningRequest, IResult>
{
    public async Task<IResult> Handle(DeleteScreeningRequest request, CancellationToken cancellationToken)
    {
        var screening = await db.Screenings.SingleOrDefaultAsync(s => s.Id == request.Id, cancellationToken);

        if (screening == null)
        {
            return Results.NotFound();
        }

        var tickets = db.Tickets.Where(t => t.Screening.Id == request.Id);

        db.Tickets.RemoveRange(tickets);
        db.Screenings.Remove(screening);

        await db.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}

public sealed class DeleteScreening : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("screenings/{id:guid}", async (
            Guid id,
            [FromServices] ISender sender,
            CancellationToken cancellationToken) =>
                await sender.Send(new DeleteScreeningRequest(id), cancellationToken))
            .WithOpenApi()
            .RequireAuthorization(ApplicationRoles.Admin)
            .Produces(204)
            .Produces(404);
    }
}
