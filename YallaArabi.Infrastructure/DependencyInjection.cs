using Microsoft.AspNetCore.Http;
using YallaArabi.Infrastructure.Ai;
using YallaArabi.Infrastructure.Images;
using YallaArabi.Infrastructure.Links;
using Microsoft.Extensions.Options;
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
        services.Configure<AppLinksOptions>(configuration.GetSection("App"));
        services.AddScoped<IAppLinks, AppLinks>();
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
        services.AddScoped<ILessonContentRepository, LessonContentRepository>();
        services.AddScoped<IUserNotificationRepository, UserNotificationRepository>();
        services.AddScoped<IAiDictionaryRepository, AiDictionaryRepository>();
        services.AddScoped<IAiModeSettingsRepository, AiModeSettingsRepository>();
        services.Configure<AiOptions>(configuration.GetSection("Ai"));
        // Claude runs only when a key is configured; otherwise the free test provider answers.
        services.AddScoped<IDialectAiProvider>(sp =>
            sp.GetRequiredService<IOptions<AiOptions>>().Value.UseClaude
                ? ActivatorUtilities.CreateInstance<ClaudeDialectAiProvider>(sp)
                : new FakeDialectAiProvider());

        services.Configure<ImageSearchOptions>(configuration.GetSection("ImageSearch"));
        services.AddHttpClient<PexelsImageSearchProvider>((sp, client) =>
        {
            client.BaseAddress = new Uri(sp.GetRequiredService<IOptions<ImageSearchOptions>>().Value.PexelsBaseUrl);
            // A slow photo lookup must never hold up the student's search.
            client.Timeout = TimeSpan.FromSeconds(5);
        });
        services.AddScoped<IImageSearchProvider>(sp =>
            sp.GetRequiredService<IOptions<ImageSearchOptions>>().Value.UsePexels
                ? sp.GetRequiredService<PexelsImageSearchProvider>()
                : new NoImageSearchProvider());
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
        services.AddScoped<IVoiceProfileRepository, VoiceProfileRepository>();
        services.AddScoped<IWordAudioRepository, WordAudioRepository>();
        services.AddScoped<IRecordingSessionRepository, RecordingSessionRepository>();
services.AddScoped<IAccountDeletionService, AccountDeletionService>();
        return services;
    }
}