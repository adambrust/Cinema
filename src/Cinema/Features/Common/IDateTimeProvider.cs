namespace Cinema.Features.Common;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
