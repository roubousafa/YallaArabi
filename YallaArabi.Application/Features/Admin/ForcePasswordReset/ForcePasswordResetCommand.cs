using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.ForcePasswordReset;

public sealed record ForcePasswordResetCommand(
    IReadOnlyList<Guid> UserIds,
    bool RevokeSessions,
    bool SendResetEmail
) : IRequest<Result<ForcePasswordResetResponse>>;

public sealed record ForcePasswordResetResponse(
    int ProcessedCount,
    int SessionsRevoked,
    int EmailsSent,
    IReadOnlyList<SkippedUser> Skipped
);

public sealed record SkippedUser(Guid UserId, string Reason);