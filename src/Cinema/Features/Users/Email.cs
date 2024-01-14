namespace Cinema.Features.Users;

public sealed record Email
{
    public string Value { get; init; }

    private Email(string value)
    {
        Value = value;
    }

    public static Email Create(string email)
    {
        return new Email(email);
    }
}
