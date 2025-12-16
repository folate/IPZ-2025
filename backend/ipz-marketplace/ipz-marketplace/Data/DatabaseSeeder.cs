using ipz_marketplace.Entities;
using Microsoft.AspNetCore.Identity;

namespace ipz_marketplace.Data;

public class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider, IWebHostEnvironment environment)
    {
        if (!environment.IsDevelopment())
        {
            return;
        }

        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        // Sprawdź czy baza jest pusta
        if (userManager.Users.Any())
        {
            return;
        }

        // Utwórz rolę Admin jeśli nie istnieje
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        // Utwórz użytkownika admin
        var adminUser = new User
        {
            UserName = "admin",
            Email = "admin@marketplace.com",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "Administrator",
            DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
