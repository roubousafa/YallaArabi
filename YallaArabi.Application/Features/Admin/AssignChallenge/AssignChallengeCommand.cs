using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.AssignChallenge;

public sealed record AssignChallengeCommand(
    Guid ChallengeId,
    IReadOnlyList<Guid>? UserIds,
    bool SendNotification
) : IRequest<Result<AssignChallengeResponse>>;

public sealed record AssignChallengeResponse(
    Guid ChallengeId,
    int AssignedCount,
    int AlreadyAssignedCount,
    int NotificationsSent,
    int NotificationsSkipped
);