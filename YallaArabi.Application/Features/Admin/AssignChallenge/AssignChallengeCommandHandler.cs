using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.AssignChallenge;

public sealed class AssignChallengeCommandHandler : IRequestHandler<AssignChallengeCommand, Result<AssignChallengeResponse>>
{
    private readonly IChallengeRepository _challengeRepository;
    private readonly IChallengeAssignmentRepository _assignmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IPushNotificationSender _pushNotificationSender;
    private readonly IUserNotificationRepository _userNotificationRepository;
    private readonly ICurrentUserService _currentUserService;

    public AssignChallengeCommandHandler(
        IChallengeRepository challengeRepository,
        IChallengeAssignmentRepository assignmentRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository,
        IPushNotificationSender pushNotificationSender,
        IUserNotificationRepository userNotificationRepository,
        ICurrentUserService currentUserService)
    {
        _challengeRepository = challengeRepository;
        _assignmentRepository = assignmentRepository;
        _userRepository = userRepository;
        _notificationRepository = notificationRepository;
        _pushNotificationSender = pushNotificationSender;
        _userNotificationRepository = userNotificationRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<AssignChallengeResponse>> Handle(AssignChallengeCommand request, CancellationToken cancellationToken)
    {
        var currentAdminId = _currentUserService.UserId;

        if (currentAdminId is null)
        {
            return Result<AssignChallengeResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var challenge = await _challengeRepository.GetByIdAsync(request.ChallengeId, cancellationToken);

        if (challenge is null)
        {
            return Result<AssignChallengeResponse>.Failure("challenge_not_found", "Challenge not found");
        }

        var targetUsers = new List<User>();

        if (request.UserIds is null || request.UserIds.Count == 0)
        {
            var allStudents = await _userRepository.GetAllByRoleAsync("Student", cancellationToken);

            targetUsers.AddRange(challenge.LevelId.HasValue
                ? allStudents.Where(u => u.CurrentLevelId == challenge.LevelId.Value)
                : allStudents);
        }
        else
        {
            foreach (var userId in request.UserIds.Distinct())
            {
                var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

                if (user is not null)
                {
                    targetUsers.Add(user);
                }
            }
        }

        if (targetUsers.Count == 0)
        {
            return Result<AssignChallengeResponse>.Failure("no_valid_recipients", "No matching students were found for this assignment");
        }

        var assignedCount = 0;
        var alreadyAssignedCount = 0;
        var notificationsSent = 0;
        var notificationsSkipped = 0;

        foreach (var user in targetUsers)
        {
            var exists = await _assignmentRepository.ExistsAsync(challenge.Id, user.Id, cancellationToken);

            if (exists)
            {
                alreadyAssignedCount++;
                continue;
            }

            var assignment = ChallengeAssignment.Create(challenge.Id, user.Id, currentAdminId.Value);
            await _assignmentRepository.AddAsync(assignment, cancellationToken);
            assignedCount++;

            if (request.SendNotification)
            {
                await _userNotificationRepository.AddRangeAsync(
                    new[]
                    {
                        UserNotification.Create(
                            user.Id,
                            NotificationTypes.ChallengeAssigned,
                            "New challenge for you!",
                            $"You have been assigned the challenge: {challenge.Title}",
                            challenge.Id)
                    },
                    cancellationToken);

                if (string.IsNullOrWhiteSpace(user.FcmToken))
                {
                    notificationsSkipped++;
                }
                else
                {
                    await _pushNotificationSender.SendCustomNotificationAsync(
                        user.FcmToken,
                        "New challenge for you!",
                        $"You have been assigned the challenge: {challenge.Title}",
                        cancellationToken);

                    notificationsSent++;
                }
            }
        }

        await _assignmentRepository.SaveChangesAsync(cancellationToken);

        if (request.SendNotification && (notificationsSent > 0 || notificationsSkipped > 0))
        {
            var notification = Notification.Create(
                "New challenge for you!",
                $"You have been assigned the challenge: {challenge.Title}",
                request.UserIds is null || request.UserIds.Count == 0 ? "all" : "specific",
                targetUsers.Count,
                notificationsSent,
                notificationsSkipped,
                currentAdminId.Value);

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await _notificationRepository.SaveChangesAsync(cancellationToken);
        }

        return Result<AssignChallengeResponse>.Success(
            new AssignChallengeResponse(
                challenge.Id,
                assignedCount,
                alreadyAssignedCount,
                notificationsSent,
                notificationsSkipped));
    }
}