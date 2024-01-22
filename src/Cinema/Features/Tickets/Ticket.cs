using Cinema.Features.Common;
using Cinema.Features.Movies;
using Cinema.Features.Sits;
using Cinema.Features.Users;

namespace Cinema.Features.Tickets;

public sealed class Ticket : IEntity
{
    public Guid Id { get; set; }
    public User User { get; set; } = null!;
    public Movie Movie { get; set; } = null!;
    public List<Sit> Sits { get; set; } = [];
}
