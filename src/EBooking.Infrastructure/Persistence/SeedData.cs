using EBooking.Application.Common;
using EBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EBooking.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task EnsureSeedData(RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();

        await EnsureUserAsync(userManager, roleManager,
        email: "appuser@ebookingapi.com",
        fullName: "EBooking AppUser",
        password: "appUser@12345",
        roleName: RoleHelper.User);

        await EnsureUserAsync(userManager, roleManager,
        email: "admin@ebookingapi.com",
        fullName: "EBooking Admin",
        password: "appAdmin@12345",
        roleName: RoleHelper.Admin);
    }

    private static async Task EnsureUserAsync(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, string email, string fullName, string password, string roleName)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user != null)
            return;

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }

        user = new ApplicationUser
        {
            FullName = fullName,
            Email = email,
            UserName = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create user '{email}': {errors}");
        }
    }
}
