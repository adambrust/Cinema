using Cinema.Features.Common;
using Cinema.Features.Movies;
using Cinema.Features.Sits;

namespace Cinema.Features.Screenings;

public sealed class Screening : IEntity
{
    public Guid Id { get; set; }
    public Movie Movie { get; set; } = null!;
    public DateTime Time { get; set; }
    public List<Sit> ReservedSits { get; set; } = [];
}
