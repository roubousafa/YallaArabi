using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Friends.GetFriends;

public sealed record GetFriendsQuery : IRequest<Result<FriendsListResponse>>;

public sealed record FriendsListResponse(
    IReadOnlyList<FriendItem> Friends,
    IReadOnlyList<PendingRequestItem> PendingRequests
);

public sealed record FriendItem(
    Guid FriendshipId,
    Guid UserId,
    string FullName,
    string? LevelCode,
    int TotalPoints,
    DateTime FriendsSince
);

public sealed record PendingRequestItem(
    Guid FriendshipId,
    Guid UserId,
    string FullName,
    string? LevelCode,
    DateTime RequestedAt
);