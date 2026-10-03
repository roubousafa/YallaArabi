using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Friends.SearchByFriendCode;

public sealed class SearchByFriendCodeQueryHandler : IRequestHandler<SearchByFriendCodeQuery, Result<FriendSearchResult>>
{
    private readonly IUserRepository _userRepository;
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly IPointsService _pointsService;
    private readonly ICurrentUserService _currentUserService;

    public SearchByFriendCodeQueryHandler(
        IUserRepository userRepository,
        IFriendshipRepository friendshipRepository,
        ILevelRepository levelRepository,
        IPointsService pointsService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _friendshipRepository = friendshipRepository;
        _levelRepository = levelRepository;
        _pointsService = pointsService;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<FriendSearchResult>> Handle(SearchByFriendCodeQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<FriendSearchResult>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var normalized = FriendCodes.Normalize(request.Code);

        if (string.IsNullOrWhiteSpace(normalized))
        {
            return Result<FriendSearchResult>.Failure("invalid_code", "Invalid friend code format");
        }

        var user = await _userRepository.GetByFriendCodeAsync(normalized, cancellationToken);

        if (user is null)
        {
            return Result<FriendSearchResult>.Failure("code_not_found", "No user found with this friend code");
        }

        if (user.Id == currentUserId.Value)
        {
            return Result<FriendSearchResult>.Failure("own_code", "This is your own friend code");
        }

        var existing = await _friendshipRepository.GetBetweenUsersAsync(currentUserId.Value, user.Id, cancellationToken);

        var relationStatus = existing switch
        {
            null => "none",
            { } f when f.IsAccepted() => "friends",
            { } f when f.IsPending() => "pending",
            _ => "none"
        };

        string? levelCode = null;

        if (user.CurrentLevelId.HasValue)
        {
            var level = await _levelRepository.GetByIdAsync(user.CurrentLevelId.Value, cancellationToken);
            levelCode = level?.Code;
        }

        var points = await _pointsService.GetTotalPointsAsync(user.Id, cancellationToken);

        return Result<FriendSearchResult>.Success(
            new FriendSearchResult(user.Id, user.FullName, levelCode, points, relationStatus));
    }
}