using Microsoft.AspNetCore.Identity;
using SurveyBasketAPI.Entities;

namespace SurveyBasketAPI.DataSeeding;

public static class CreateUser
{
    public static async Task SeedUserAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var existingUser = await userManager.FindByEmailAsync("mohamedmagdy@gmail.com");
        if (existingUser != null)
            return;

        var user = new ApplicationUser
        {
            UserName = "mohamedmagdy@gmail.com",
            FirstName = "Mohamed",
            LastName = "Magdy",
            Email = "mohamedmagdy@gmail.com"
        };

        await userManager.CreateAsync(user, "Password123!");
    }
}
