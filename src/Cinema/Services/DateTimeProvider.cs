using Cinema.Features.Common;

namespace Cinema.Persistance;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
