using Microsoft.Extensions.Logging;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Notifications;

public class ConsolePushNotificationSender : IPushNotificationSender
{
    private readonly ILogger<ConsolePushNotificationSender> _logger;

    public ConsolePushNotificationSender(ILogger<ConsolePushNotificationSender> logger) => _logger = logger;

    public Task SendReviewReminderAsync(string fcmToken, string studentName, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[DEV PUSH NOTIFICATION] To: {StudentName} (token: {Token}) — \"Time to review your words! You have overdue reviews waiting.\"",
            studentName, fcmToken);

        return Task.CompletedTask;
    }
}