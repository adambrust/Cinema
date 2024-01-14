using Cinema.Features.Common;

namespace Cinema.Features.Movies;

public sealed class Movie : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public TimeSpan Duration { get; private set; }

    private Movie(Guid id, string title, string description, TimeSpan duration) : base(id)
    {
        Title = title;
        Description = description;
        Duration = duration;
    }

    public static Movie Create(string title, string description, TimeSpan duration)
    {
        return new Movie(Guid.NewGuid(), title, description, duration);
    }

    public void Update(string title, string description, TimeSpan duration)
    {
        Title = title;
        Description = description;
        Duration = duration;
    }
}
