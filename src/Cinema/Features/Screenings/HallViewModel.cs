namespace Cinema.Features.Screenings;

public sealed record HallViewModel(Guid Id, int Number);

public static class HallViewModelExtensions
{
    public static HallViewModel ToViewModel(this Hall hall)
    {
        return new(hall.Id, hall.Number);
    }

    public static IEnumerable<HallViewModel> ToViewModel(this IEnumerable<Hall> halls)
    {
        var models = new List<HallViewModel>();
        foreach (var hall in halls)
        {
            models.Add(hall.ToViewModel());
        }
        return models;
    }
}
