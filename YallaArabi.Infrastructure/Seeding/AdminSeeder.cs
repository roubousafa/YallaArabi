using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Seeding;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var userRepository = scope.ServiceProvider.GetRequiredService<IUserRepository>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<object>>();

        var adminEmail = configuration["AdminSeed:Email"];
        var adminPassword = configuration["AdminSeed:Password"];
        var adminFullName = configuration["AdminSeed:FullName"] ?? "System Admin";

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
        {
            logger.LogWarning("AdminSeed configuration is missing. Skipping admin user seeding.");
            return;
        }

        var existingUser = await userRepository.GetByEmailAsync(adminEmail);

        if (existingUser is not null)
        {
            if (existingUser.Role != "Admin")
            {
                existingUser.PromoteToAdmin();
                await userRepository.SaveChangesAsync();
                logger.LogInformation("Existing user {Email} promoted to Admin.", adminEmail);
            }

            return;
        }

        var passwordHash = passwordHasher.Hash(adminPassword);
        var adminUser = User.Register(adminFullName, adminEmail, passwordHash);
        adminUser.PromoteToAdmin();

        await userRepository.AddAsync(adminUser);

        logger.LogInformation("Admin user {Email} created successfully.", adminEmail);
    }
}