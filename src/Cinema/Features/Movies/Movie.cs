using Cinema.Features.Common;

namespace Cinema.Features.Movies;

public sealed class Movie(
    Guid id,
    string title,
    string description,
    TimeSpan duration,
    string image)
    : IEntity
{
    public Guid Id { get; set; } = id;
    public string Title { get; set; } = title;
    public string Description { get; set; } = description;
    public TimeSpan Duration { get; set; } = duration;
    public string Image { get; set; } = image;
}
