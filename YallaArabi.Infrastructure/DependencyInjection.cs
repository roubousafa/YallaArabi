using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YallaArabi.Application.Interfaces;
using YallaArabi.Infrastructure.Authentication;
using YallaArabi.Infrastructure.Emailing;
using YallaArabi.Infrastructure.Persistence;
using YallaArabi.Infrastructure.Repositories;
using YallaArabi.Infrastructure.Security;
using YallaArabi.Infrastructure.Services;

namespace YallaArabi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.Configure<GoogleOptions>(configuration.GetSection("Google"));
        services.Configure<EmailOptions>(configuration.GetSection("Email"));

        services.AddHttpContextAccessor();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ILevelRepository, LevelRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IStudentLessonProgressRepository, StudentLessonProgressRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IEmailSender, BrevoEmailSender>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IGoogleAuthValidator, GoogleAuthValidator>();
        services.AddScoped<IWordRepository, WordRepository>();

        return services;
    }
}