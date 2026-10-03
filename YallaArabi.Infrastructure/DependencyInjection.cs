using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YallaArabi.Application.Interfaces;
using YallaArabi.Infrastructure.Authentication;
using YallaArabi.Infrastructure.Emailing;
using YallaArabi.Infrastructure.Notifications;
using YallaArabi.Infrastructure.Parsing;
using YallaArabi.Infrastructure.Payments;
using YallaArabi.Infrastructure.Persistence;
using YallaArabi.Infrastructure.Repositories;
using YallaArabi.Infrastructure.Security;
using YallaArabi.Infrastructure.Services;
using YallaArabi.Infrastructure.Storage;

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
        services.Configure<FirebaseOptions>(configuration.GetSection("Firebase"));

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
        services.AddScoped<ISavedWordRepository, SavedWordRepository>();
        services.AddScoped<IWordReviewRepository, WordReviewRepository>();
        services.AddScoped<IPushNotificationSender, FirebasePushNotificationSender>();
        services.AddScoped<IExcelWordParser, ExcelWordParser>();
        services.AddScoped<IPaymentGateway, FakePaymentGateway>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IExcelTemplateGenerator, ExcelTemplateGenerator>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IPointRuleRepository, PointRuleRepository>();
        services.AddScoped<IPointTransactionRepository, PointTransactionRepository>();
        services.AddScoped<IUserActivityRepository, UserActivityRepository>();
        services.AddScoped<IClientTimeZoneProvider, HeaderTimeZoneProvider>();
        services.AddScoped<IChallengeRepository, ChallengeRepository>();
        services.AddScoped<IChallengeQuestionRepository, ChallengeQuestionRepository>();
        services.AddScoped<IChallengeParticipationRepository, ChallengeParticipationRepository>();
        services.AddScoped<IFriendshipRepository, FriendshipRepository>();
        services.Configure<FileStorageOptions>(configuration.GetSection("FileStorage"));
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IChallengeAssignmentRepository, ChallengeAssignmentRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IChallengeAnswerRepository, ChallengeAnswerRepository>();
        return services;
    }
}