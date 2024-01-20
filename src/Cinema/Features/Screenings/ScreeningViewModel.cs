using Cinema.Features.Sits;

namespace Cinema.Features.Screenings;

public sealed record ScreeningViewModel(
    Guid Id,
    Guid MovieId,
    DateTime Time,
    List<Sit> ReservedSits);

public static class ScreeningViewModelExtensions
{
    public static ScreeningViewModel ToViewModel(this Screening screening)
    {
        return new(
            screening.Id,
            screening.Movie.Id,
            screening.Time,
            screening.ReservedSits);
    }

    public static IEnumerable<ScreeningViewModel> ToViewModel(this IEnumerable<Screening> screenings)
    {
        var models = new List<ScreeningViewModel>();
        foreach (var screening in screenings)
        {
            models.Add(screening.ToViewModel());
        }
        return models;
    }
}
