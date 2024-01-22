namespace Cinema.Features.Movies;

public sealed record MovieViewModel(
    Guid Id,
    string Title,
    string Description,
    DateTime Time,
    string Image,
    IEnumerable<Guid> ReservedSits);

public sealed record MovieListViewModel(
    Guid Id,
    string Title,
    string Description,
    DateTime Time,
    string Image);

public static class MovieViewModelExtensions
{
    public static MovieViewModel ToViewModel(this Movie movie)
    {
        return new(
            movie.Id,
            movie.Title,
            movie.Description,
            movie.Time,
            movie.Image,
            movie.ReservedSits.Select(s => s.Id));
    }

    public static IEnumerable<MovieListViewModel> ToViewModel(this IEnumerable<Movie> movies)
    {
        var models = new List<MovieListViewModel>();
        foreach (var movie in movies)
        {
            models.Add(new(
                movie.Id,
                movie.Title,
                movie.Description,
                movie.Time,
                movie.Image
            ));
        }
        return models;
    }
}
