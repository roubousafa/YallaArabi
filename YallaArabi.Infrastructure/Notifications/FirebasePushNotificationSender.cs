using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Notifications;

public class FirebasePushNotificationSender : IPushNotificationSender
{
    private static readonly object InitLock = new();
    private static bool _initialized;

    private readonly FirebaseOptions _options;
    private readonly ILogger<FirebasePushNotificationSender> _logger;

    public FirebasePushNotificationSender(
        IOptions<FirebaseOptions> options,
        ILogger<FirebasePushNotificationSender> logger)
    {
        _options = options.Value;
        _logger = logger;

        EnsureInitialized();
    }

    private void EnsureInitialized()
    {
        if (_initialized || !_options.Enabled)
        {
            return;
        }

        lock (InitLock)
        {
            if (_initialized)
            {
                return;
            }

            try
            {
                var keyPath = Path.Combine(AppContext.BaseDirectory, _options.ServiceAccountKeyPath);

                if (!File.Exists(keyPath))
                {
                    _logger.LogError("Firebase service account key file not found at {Path}. Push notifications are disabled.", keyPath);
                    return;
                }

                if (FirebaseApp.DefaultInstance is null)
                {
                    FirebaseApp.Create(new AppOptions
                    {
                        Credential = GoogleCredential.FromFile(keyPath)
                    });
                }

                _initialized = true;
                _logger.LogInformation("Firebase Admin SDK initialized successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to initialize Firebase Admin SDK. Push notifications are disabled.");
            }
        }
    }

    public async Task SendReviewReminderAsync(string fcmToken, string studentName, CancellationToken ct = default)
    {
        await SendAsync(
            fcmToken,
            "Time to review!",
            $"Hi {studentName}, you have words waiting for review. Keep your streak going!",
            ct);
    }

    public async Task SendCustomNotificationAsync(string fcmToken, string title, string body, CancellationToken ct = default)
    {
        await SendAsync(fcmToken, title, body, ct);
    }

    private async Task SendAsync(string fcmToken, string title, string body, CancellationToken ct)
    {
        if (!_options.Enabled || !_initialized)
        {
            _logger.LogInformation(
                "[PUSH SKIPPED] Firebase is not enabled or not initialized. Title: \"{Title}\"",
                title);
            return;
        }

        if (string.IsNullOrWhiteSpace(fcmToken))
        {
            return;
        }

        var message = new Message
        {
            Token = fcmToken,
            Notification = new Notification
            {
                Title = title,
                Body = body
            }
        };

        try
        {
            var messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message, ct);
            _logger.LogInformation("Push notification sent successfully. MessageId: {MessageId}", messageId);
        }
        catch (FirebaseMessagingException ex)
        {
            _logger.LogWarning(ex, "Failed to send push notification to token {Token}", fcmToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while sending push notification");
        }
    }
}