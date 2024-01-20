using Cinema.Features.Movies;

namespace Cinema.Features.Screenings;

public sealed record ScreeningViewModel(
    Guid Id,
    MovieViewModel Movie,
    HallViewModel Hall,
    List<Sit> Sits,
    DateTime Time,
    List<Sit> ReservedSits);

public sealed record ScreeningListViewModel(
    Guid Id,
    MovieViewModel Movie,
    HallViewModel Hall,
    DateTime Time);

public static class ScreeningViewModelExtensions
{
    public static ScreeningViewModel ToViewModel(this Screening screening)
    {
        return new(
            screening.Id,
            screening.Movie.ToViewModel(),
            screening.Hall.ToViewModel(),
            screening.Hall.Sits,
            screening.Time,
            screening.ReservedSits);
    }

    public static IEnumerable<ScreeningListViewModel> ToViewModel(this IEnumerable<Screening> screenings)
    {
        var models = new List<ScreeningListViewModel>();
        foreach (var screening in screenings)
        {
            models.Add(new(
                screening.Id,
                screening.Movie.ToViewModel(),
                screening.Hall.ToViewModel(),
                screening.Time));
        }
        return models;
    }
}
