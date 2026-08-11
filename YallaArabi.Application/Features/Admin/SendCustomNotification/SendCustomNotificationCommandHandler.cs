using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.SendCustomNotification;

public sealed class SendCustomNotificationCommandHandler : IRequestHandler<SendCustomNotificationCommand, Result<SendCustomNotificationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly ICurrentUserService _currentUserService;

    public SendCustomNotificationCommandHandler(
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        IPushNotificationSender pushNotificationSender,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _pushNotificationSender = pushNotificationSender;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SendCustomNotificationResponse>> Handle(SendCustomNotificationCommand request, CancellationToken cancellationToken)
    {
        var currentAdminId = _currentUserService.UserId;

        if (currentAdminId is null)
        {
            return Result<SendCustomNotificationResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var recipients = new List<User>();
        string targetType;

        if (request.UserIds is null || request.UserIds.Count == 0)
        {
            recipients.AddRange(await _userRepository.GetAllByRoleAsync("Student", cancellationToken));
            targetType = "all";
        }
        else
        {
            targetType = "specific";

            foreach (var userId in request.UserIds)
            {
                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

                if (user is not null)
                {
                    recipients.Add(user);
                }
            }

            if (recipients.Count == 0)
            {
                return Result<SendCustomNotificationResponse>.Failure("no_valid_recipients", "None of the specified users were found");
            }
        }

        var deliveredCount = 0;
        var skippedCount = 0;

        foreach (var recipient in recipients)
        {
            if (string.IsNullOrWhiteSpace(recipient.FcmToken))
            {
                skippedCount++;
                continue;
            }

            await _pushNotificationSender.SendCustomNotificationAsync(
                recipient.FcmToken, request.Title, request.Body, cancellationToken);

            deliveredCount++;
        }

        var notification = Notification.Create(
            request.Title,
            request.Body,
            targetType,
            recipients.Count,
            deliveredCount,
            skippedCount,
            currentAdminId.Value);

        await _notificationRepository.AddAsync(notification, cancellationToken);
        await _notificationRepository.SaveChangesAsync(cancellationToken);

        return Result<SendCustomNotificationResponse>.Success(
            new SendCustomNotificationResponse(notification.Id, recipients.Count, deliveredCount, skippedCount));
    }
}