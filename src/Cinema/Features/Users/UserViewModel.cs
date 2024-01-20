namespace Cinema.Features.Users;

public sealed record UserViewModel(Guid Id, string Email);

public static class UserViewModelExtensions
{
    public static UserViewModel ToViewModel(this User user)
    {
        return new(user.Id, user.Email ?? string.Empty);
    }
}
