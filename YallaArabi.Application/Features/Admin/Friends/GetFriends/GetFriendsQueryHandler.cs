using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Friends.GetFriends;

public sealed class GetFriendsQueryHandler : IRequestHandler<GetFriendsQuery, Result<FriendsListResponse>>
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly IPointsService _pointsService;
    private readonly ICurrentUserService _currentUserService;

    public GetFriendsQueryHandler(
        IFriendshipRepository friendshipRepository,
        IUserRepository userRepository,
        ILevelRepository levelRepository,
        IPointsService pointsService,
        ICurrentUserService currentUserService)
    {
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
        _levelRepository = levelRepository;
        _pointsService = pointsService;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<FriendsListResponse>> Handle(GetFriendsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<FriendsListResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        var accepted = await _friendshipRepository.GetAcceptedForUserAsync(currentUserId.Value, cancellationToken);
        var friends = new List<FriendItem>();

        foreach (var friendship in accepted)
        {
            var friendId = friendship.GetOtherUserId(currentUserId.Value);
            var friendUser = await _userRepository.GetByIdAsync(friendId, cancellationToken);

            if (friendUser is null)
            {
                continue;
            }

            var points = await _pointsService.GetTotalPointsAsync(friendId, cancellationToken);

            friends.Add(new FriendItem(
                friendship.Id,
                friendUser.Id,
                friendUser.FullName,
                friendUser.CurrentLevelId.HasValue && levelCodeById.TryGetValue(friendUser.CurrentLevelId.Value, out var code) ? code : null,
                points,
                friendship.RespondedAt ?? friendship.RequestedAt));
        }

        var pending = await _friendshipRepository.GetPendingRequestsForUserAsync(currentUserId.Value, cancellationToken);
        var pendingItems = new List<PendingRequestItem>();

        foreach (var friendship in pending)
        {
            var requesterUser = await _userRepository.GetByIdAsync(friendship.RequesterId, cancellationToken);

            if (requesterUser is null)
            {
                continue;
            }

            pendingItems.Add(new PendingRequestItem(
                friendship.Id,
                requesterUser.Id,
                requesterUser.FullName,
                requesterUser.CurrentLevelId.HasValue && levelCodeById.TryGetValue(requesterUser.CurrentLevelId.Value, out var code) ? code : null,
                friendship.RequestedAt));
        }

        return Result<FriendsListResponse>.Success(new FriendsListResponse(friends, pendingItems));
    }
}