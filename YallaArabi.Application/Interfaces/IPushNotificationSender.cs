namespace YallaArabi.Application.Interfaces;

public interface IPushNotificationSender
{
    Task SendReviewReminderAsync(string fcmToken, string studentName, CancellationToken ct = default);
    Task SendCustomNotificationAsync(string fcmToken, string title, string body, CancellationToken ct = default);
}