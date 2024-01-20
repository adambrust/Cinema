using Cinema.Features.Common;

namespace Cinema.Features.Sits;

public sealed class Sit : IEntity
{
    public Guid Id { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
}
