using Cinema.Features.Screenings;
using Cinema.Features.Users;

namespace Cinema.Features.Tickets;

public sealed record TicketViewModel(
    Guid Id,
    UserViewModel User,
    ScreeningViewModel Screening,
    List<Sit> Sits);

public static class TicketViewModelExtensions
{
    public static TicketViewModel ToViewModel(this Ticket ticket)
    {
        return new(ticket.Id, ticket.User.ToViewModel(), ticket.Screening.ToViewModel(), ticket.Sits);
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
