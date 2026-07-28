using Mediator;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Notifications.SendReminderNotifications;

public sealed class SendReminderNotificationsCommandHandler : IRequestHandler<SendReminderNotificationsCommand, SendReminderNotificationsResponse>
{
    private const int OverdueThresholdDays = 7;

    private readonly IWordReviewRepository _wordReviewRepository;
    private readonly IUserRepository _userRepository;
    private readonly IPushNotificationSender _pushNotificationSender;

    public SendReminderNotificationsCommandHandler(
        IWordReviewRepository wordReviewRepository,
        IUserRepository userRepository,
        IPushNotificationSender pushNotificationSender)
    {
        _wordReviewRepository = wordReviewRepository;
        _userRepository = userRepository;
        _pushNotificationSender = pushNotificationSender;
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