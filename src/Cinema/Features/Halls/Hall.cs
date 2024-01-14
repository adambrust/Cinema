using Cinema.Features.Common;

namespace Cinema.Features.Halls;

public sealed class Hall : Entity
{
    private readonly int[] _sits = [];

    public int Number { get; private init; }
    public int NumberOfRows => _sits.Length;

    public int GetNumberOfSitsInRow(int row) => _sits[row];

    private Hall(Guid id, int number, int[] sits) : base(id)
    {
        Number = number;
        _sits = sits;
    }
}
