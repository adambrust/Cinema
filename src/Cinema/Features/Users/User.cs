using Cinema.Features.Common;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Features.Users;

public sealed class User : IdentityUser<Guid>, IEntity
{
}
