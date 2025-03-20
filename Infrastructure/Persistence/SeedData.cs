using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Persistence;

public static class SeedData
{
    public static async Task SeedRolesAsync(RoleManager<UserRole> roleManager)
    {
        string[] roleNames = ["Admin", "Owner", "Manager", "Worker"];

        foreach (var role in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new UserRole { Name = role });
                Console.WriteLine($"{role} created");
            }
            else Console.WriteLine($"{role} already exists");
        }
    }
}

// Paste in the program.cs file before app.Run()
/*using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Get the RoleManager instance from DI
        var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
        await SeedData.SeedRolesAsync(roleManager);

        // Optionally, seed default users here as well:
        // var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        // await SeedData.SeedUsersAsync(userManager);
    }
    catch (Exception ex)
    {
        // Log the error (or handle accordingly)
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}*/