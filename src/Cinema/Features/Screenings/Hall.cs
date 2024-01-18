using Cinema.Features.Common;

namespace Cinema.Features.Screenings;

public sealed class Hall(Guid id, int number, HashSet<Sit> sits) : IEntity
{
    public Guid Id { get; set; } = id;
    public int Number { get; set; } = number;
    public HashSet<Sit> Sits { get; set; } = sits;
}
