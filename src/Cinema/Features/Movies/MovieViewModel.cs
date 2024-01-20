namespace Cinema.Features.Movies;

public sealed record MovieViewModel(
    Guid Id,
    string Title,
    string Description,
    int Duration,
    string Image);

public static class MovieViewModelExtensions
{
    public static MovieViewModel ToViewModel(this Movie movie)
    {
        return new(
            movie.Id,
            movie.Title,
            movie.Description,
            movie.Duration.Minutes,
            movie.Image);
    }

    public static IEnumerable<MovieViewModel> ToViewModel(this IEnumerable<Movie> movies)
    {
        var models = new List<MovieViewModel>();
        foreach (var movie in movies)
        {
            models.Add(movie.ToViewModel());
        }
        return models;
    }
}
