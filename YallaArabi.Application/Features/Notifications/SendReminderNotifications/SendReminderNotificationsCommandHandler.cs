using Mediator;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Notifications.SendReminderNotifications;

public sealed class SendReminderNotificationsCommandHandler : IRequestHandler<SendReminderNotificationsCommand, SendReminderNotificationsResponse>
{
    private const int OverdueThresholdDays = 7;
    private const string ReminderTitle = "Time to review!";
    private const string ReminderBody = "You have words waiting for review. Keep your streak going!";

    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly IUserNotificationRepository _userNotificationRepository;

    public SendReminderNotificationsCommandHandler(
        IWordReviewRepository wordReviewRepository,
        IUserRepository userRepository,
        IPushNotificationSender pushNotificationSender,
        IUserNotificationRepository userNotificationRepository)
    {
        _wordReviewRepository = wordReviewRepository;
        _userRepository = userRepository;
        _pushNotificationSender = pushNotificationSender;
        _userNotificationRepository = userNotificationRepository;
    }

    public async ValueTask<SendReminderNotificationsResponse> Handle(SendReminderNotificationsCommand request, CancellationToken cancellationToken)
    {
        var overdueBefore = DateTime.UtcNow.AddDays(-OverdueThresholdDays);
        var studentIds = await _wordReviewRepository.GetUserIdsWithOverdueReviewsAsync(overdueBefore, cancellationToken);

        var sentCount = 0;
        var skippedCount = 0;

        foreach (var studentId in studentIds)
        {
            var student = await _userRepository.GetByIdAsync(studentId, cancellationToken);

            // The reminder job may run several times a day; keep the inbox to one reminder per day.
            if (student is not null
                && !await _userNotificationRepository.ExistsSinceAsync(student.Id, NotificationTypes.ReviewReminder, DateTime.UtcNow.AddHours(-20), cancellationToken))
            {
                await _userNotificationRepository.AddRangeAsync(
                    new[] { UserNotification.Create(student.Id, NotificationTypes.ReviewReminder, ReminderTitle, ReminderBody) },
                    cancellationToken);
                await _userNotificationRepository.SaveChangesAsync(cancellationToken);
            }

            if (student?.FcmToken is null)
            {
                skippedCount++;
                continue;
            }

            await _pushNotificationSender.SendReviewReminderAsync(student.FcmToken, student.FullName, cancellationToken);
            sentCount++;
        }

        return new SendReminderNotificationsResponse(sentCount, skippedCount);
    }
}