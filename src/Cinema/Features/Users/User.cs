using Cinema.Features.Common;

namespace Cinema.Features.Users;

public sealed class User : Entity
{
    public string Name { get; private set; }
    public string LastName { get; private set; }
    public Email Email { get; private set; }
    public UserRoles Roles { get; private set; }

    private User(Guid id, string name, string lastName, Email email, UserRoles roles) : base(id)
    {
        Name = name;
        LastName = lastName;
        Email = email;
        Roles = roles;
    }

    public static User Create(string? firstName, string? lastName, Email email, UserRoles roles)
    {
        return new User(Guid.NewGuid(), firstName, lastName, email, roles);
    }
}

