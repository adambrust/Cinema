using Cinema.Features.Sits;

namespace Cinema.Features.Tickets;

public sealed record TicketViewModel(
    Guid Id,
    Guid UserId,
    Guid ScreeningId,
    List<Sit> Sits);

public static class TicketViewModelExtensions
{
    public static TicketViewModel ToViewModel(this Ticket ticket)
    {
        return new(ticket.Id, ticket.User.Id, ticket.Screening.Id, ticket.Sits);
    }

    public static IEnumerable<TicketViewModel> ToViewModel(this IEnumerable<Ticket> tickets)
    {
        var models = new List<TicketViewModel>();
        foreach (var ticket in tickets)
        {
            models.Add(ticket.ToViewModel());
        }
        return models;
    }
}
