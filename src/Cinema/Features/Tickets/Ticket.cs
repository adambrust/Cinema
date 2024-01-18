using Cinema.Features.Common;
using Cinema.Features.Screenings;
using Cinema.Features.Users;

namespace Cinema.Features.Tickets;

public sealed class Ticket(Guid id, User user, Screening screening, HashSet<Sit> sits) : IEntity
{
    public Guid Id { get; set; } = id;
    public User User { get; set; } = user;
    public Screening Screening { get; set; } = screening;
    public HashSet<Sit> Sits { get; set; } = sits;
}
