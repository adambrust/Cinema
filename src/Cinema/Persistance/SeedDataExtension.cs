using Cinema.Features.Screenings;
using Cinema.Features.Users;
using Microsoft.AspNetCore.Identity;

namespace Cinema.Persistance;

public static class SeedDataExtension
{
    public static async Task SeedData(this IHost app, InitialAdmin initialAdmin)
    {
        using var scope = app.Services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var roles = new[] { ApplicationRoles.Admin, ApplicationRoles.Worker };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new(role));
            }
        }

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        if (await userManager.FindByEmailAsync(initialAdmin.Email) is null)
        {
            var user = new User()
            {
                UserName = initialAdmin.Email,
                Email = initialAdmin.Email
            };

            await userManager.CreateAsync(user, initialAdmin.Password);

            await userManager.AddToRoleAsync(user, ApplicationRoles.Admin);
        }

        var db = scope.ServiceProvider.GetRequiredService<CinemaDbContext>();

        if (db.Halls.Any())
        {
            return;
        }

        var sits = new List<Sit>();
        for (var i = 1; i <= 10; i++)
        {
            for (int j = 1; j <= 15; j++)
            {
                sits.Add(new(i, j));
            }
        }

        var halls = new List<Hall>();
        for (var i = 1; i <= 10; i++)
        {
            halls.Add(new()
            {
                Id = Guid.NewGuid(),
                Number = i,
                Sits = sits
            });
        }

        await db.Halls.AddRangeAsync(halls);

        await db.SaveChangesAsync();
    }
}
