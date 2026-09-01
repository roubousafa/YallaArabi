using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Seeding;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<object>>();

        try
        {
            var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            var adminEmail = configuration["AdminSeed:Email"];
            var adminPassword = configuration["AdminSeed:Password"];
            var adminFullName = configuration["AdminSeed:FullName"] ?? "System Admin";

            if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
            {
                logger.LogWarning("AdminSeed configuration is missing. Skipping admin user seeding.");
                return;
            }

            var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository>();

            var superAdminRole = await roleRepository.GetByNameEnAsync(Permissions.SuperAdminRoleName);

            if (superAdminRole is null)
            {
                superAdminRole = Role.Create(
                    Permissions.SuperAdminRoleName,
                    "مدير عام",
                    "Full access to every feature. This role cannot be modified or deleted.",
                    isSystemRole: true);

                await roleRepository.AddAsync(superAdminRole);
                await roleRepository.SaveChangesAsync();

                logger.LogInformation("Super Admin role created.");
            }

            var existingUser = await userRepository.GetByEmailAsync(adminEmail);

            if (existingUser is not null)
            {
                var changed = false;

                if (existingUser.Role != "Admin")
                {
                    existingUser.PromoteToAdmin();
                    changed = true;
                }

                if (existingUser.RoleId != superAdminRole.Id)
                {
                    existingUser.AssignRole(superAdminRole.Id);
                    changed = true;
                }

                if (changed)
                {
                    await userRepository.SaveChangesAsync();
                    logger.LogInformation("Existing user {Email} linked to Super Admin role.", adminEmail);
                }

                return;
            }

            var passwordHash = passwordHasher.Hash(adminPassword);
            var adminUser = User.Register(adminFullName, adminEmail, passwordHash);
            adminUser.PromoteToAdmin();
            adminUser.AssignRole(superAdminRole.Id);

            await userRepository.AddAsync(adminUser);

            logger.LogInformation("Admin user {Email} created successfully.", adminEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Admin seeding failed (database may be unavailable). The application will continue to start.");
        }
    }
}