using Cinema.Features.Common;
using Cinema.Features.Sits;

namespace Cinema.Features.Movies;

public sealed class Movie : IEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public string Image { get; set; } = string.Empty;
    public List<Sit> ReservedSits { get; set; } = [];
}
