using Microsoft.AspNetCore.Identity;
using MinimalAPITemplate.Data;
using MinimalAPITemplate.Entities;

namespace MinimalAPITemplate.Extensions
{
    public static class SeedExtensions
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roles = new string[] { "Owner", "Administrator", "Manager", "Editor", "Buyer", "Business", "Seller", "Subscriber" };

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (!context.Users.Any())
            {
                var user = new ApplicationUser
                {
                    FirstName = Environment.GetEnvironmentVariable("ADMIN_USERNAME"),
                    LastName = "User",
                    Email = Environment.GetEnvironmentVariable("ADMIN_EMAIL")?.ToLowerInvariant(),
                    UserName = Environment.GetEnvironmentVariable("ADMIN_EMAIL")?.ToLowerInvariant(),
                    Handle = Environment.GetEnvironmentVariable("ADMIN_USERNAME")?.ToLowerInvariant(),
                    PhoneNumber = "+111111111111",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };

                var password = Environment.GetEnvironmentVariable("ADMIN_PASSWORD")!;
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Owner"); 
                }
                else
                {
                    throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
