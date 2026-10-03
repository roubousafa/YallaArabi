using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Friends.SearchByFriendCode;

public sealed record SearchByFriendCodeQuery(string Code) : IRequest<Result<FriendSearchResult>>;

public sealed record FriendSearchResult(
    Guid UserId,
    string FullName,
    string? LevelCode,
    int TotalPoints,
    string RelationStatus
);