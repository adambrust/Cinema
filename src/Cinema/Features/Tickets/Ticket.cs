using Cinema.Features.Common;
using Cinema.Features.Screenings;

namespace Cinema.Features.Tickets;

public sealed class Ticket : Entity
{
    public Guid UserId { get; private set; }
    public Guid ShowId { get; private set; }
    public List<Sit> Sits { get; private set; }
    public TicketStatus Status { get; private set; }

    private Ticket(Guid id, Guid userId, Guid showId, List<Sit> sits, TicketStatus status) : base(id)
    {
        UserId = userId;
        ShowId = showId;
        Sits = sits;
        Status = status;
    }

    public static Ticket Create(Guid userId, Guid showId, List<Sit> sits)
    {
        return new Ticket(Guid.NewGuid(), userId, showId, sits, TicketStatus.Active);
    }

    public void Cancel()
    {
        Status = TicketStatus.Canceled;
    }
}
