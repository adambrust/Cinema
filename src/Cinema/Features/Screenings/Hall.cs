using Cinema.Features.Common;

namespace Cinema.Features.Screenings;

public sealed class Hall : IEntity
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public List<Sit> Sits { get; set; } = [];
}
