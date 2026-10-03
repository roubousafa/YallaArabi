using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Friends.RemoveFriend;

public sealed class RemoveFriendCommandHandler : IRequestHandler<RemoveFriendCommand, Result<Unit>>
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveFriendCommandHandler(
        IFriendshipRepository friendshipRepository,
        ICurrentUserService currentUserService)
    {
        _friendshipRepository = friendshipRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<Unit>> Handle(RemoveFriendCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<Unit>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var friendship = await _friendshipRepository.GetByIdAsync(request.FriendshipId, cancellationToken);

        if (friendship is null)
        {
            return Result<Unit>.Failure("friendship_not_found", "Friendship not found");
        }

        if (friendship.RequesterId != currentUserId.Value && friendship.AddresseeId != currentUserId.Value)
        {
            return Result<Unit>.Failure("forbidden", "This friendship does not involve you");
        }

        await _friendshipRepository.RemoveAsync(friendship, cancellationToken);
        await _friendshipRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}