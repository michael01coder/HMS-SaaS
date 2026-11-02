using HMS.AuthService.Domain.Data;
using Microsoft.AspNetCore.Identity;

namespace HMS.AuthService.Api.Configurations
{
    public static class SuperAdminSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

            var email = config["SEED_SUPERADMIN_EMAIL"];
            var password = config["SEED_SUPERADMIN_PASSWORD"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

            var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            if (await users.FindByEmailAsync(email) != null) return;

            const string roleName = "SuperAdmin";
            if (!await roles.RoleExistsAsync(roleName))
            {
                await roles.CreateAsync(new IdentityRole<Guid>
                {
                    Id = Guid.NewGuid(),
                    Name = roleName,
                    NormalizedName = roleName.ToUpperInvariant()
                });
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = email,
                Email = email,
                FirstName = config["SEED_SUPERADMIN_FIRST_NAME"] ?? "Platform",
                LastName = config["SEED_SUPERADMIN_LAST_NAME"] ?? "Admin",
                IsActive = true,
                EmailConfirmed = true
            };

            var created = await users.CreateAsync(user, password);
            if (!created.Succeeded)
            {
                logger.LogError("SuperAdmin seed failed: {Errors}", string.Join(", ", created.Errors.Select(e => e.Description)));
                return;
            }

            await users.AddToRoleAsync(user, roleName);
            logger.LogInformation("SuperAdmin {Email} created.", email);
        }
    }
}
