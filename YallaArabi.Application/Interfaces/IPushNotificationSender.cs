namespace YallaArabi.Application.Interfaces;

public interface IPushNotificationSender
{
    Task SendReviewReminderAsync(string fcmToken, string studentName, CancellationToken ct = default);
}