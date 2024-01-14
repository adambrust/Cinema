using Cinema.Features.Common;
using Cinema.Features.Halls;
using Cinema.Features.Movies;

namespace Cinema.Features.Screenings;

public sealed class Screening : Entity
{
    private readonly HashSet<Sit> _reservedSits;

    public Movie Movie { get; private set; }
    public Hall Hall { get; private set; }
    public DateTime Time { get; private set; }

    public IReadOnlySet<Sit> ReservedSits => _reservedSits;

    private Screening(Guid id, Movie movie, Hall hall, DateTime time) : base(id)
    {
        Movie = movie;
        Hall = hall;
        Time = time;
        _reservedSits = [];
    }

    public static Screening Create(Movie movie, Hall hall, DateTime time)
    {
        return new Screening(Guid.NewGuid(), movie, hall, time);
    }

    public bool Reserve(Sit sit)
    {
        return _reservedSits.Add(sit);
    }
}
