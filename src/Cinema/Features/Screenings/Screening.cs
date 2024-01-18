using Cinema.Features.Common;
using Cinema.Features.Movies;

namespace Cinema.Features.Screenings;

public sealed class Screening(
    Guid id,
    Movie movie,
    Hall hall,
    DateTime time,
    HashSet<Sit> reservedSits)
    : IEntity
{
    public Guid Id { get; set; } = id;
    public Movie Movie { get; set; } = movie;
    public Hall Hall { get; set; } = hall;
    public DateTime Time { get; set; } = time;
    public HashSet<Sit> ReservedSits { get; set; } = reservedSits;
}
