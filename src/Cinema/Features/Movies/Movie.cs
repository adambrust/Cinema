using Cinema.Features.Common;

namespace Cinema.Features.Movies;

public sealed class Movie : IEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TimeSpan Duration { get; set; }
    public string Image { get; set; } = string.Empty;
}
