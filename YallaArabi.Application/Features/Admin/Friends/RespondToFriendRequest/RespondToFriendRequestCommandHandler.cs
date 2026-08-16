using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Friends.RespondToFriendRequest;

public sealed class RespondToFriendRequestCommandHandler : IRequestHandler<RespondToFriendRequestCommand, Result<Unit>>
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly ICurrentUserService _currentUserService;

    public RespondToFriendRequestCommandHandler(
        IFriendshipRepository friendshipRepository,
        ICurrentUserService currentUserService)
    {
        _friendshipRepository = friendshipRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<Unit>> Handle(RespondToFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<Unit>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var friendship = await _friendshipRepository.GetByIdAsync(request.FriendshipId, cancellationToken);

        if (friendship is null)
        {
            return Result<Unit>.Failure("request_not_found", "Friend request not found");
        }

        if (friendship.AddresseeId != currentUserId.Value)
        {
            return Result<Unit>.Failure("forbidden", "You can only respond to friend requests sent to you");
        }

        if (!friendship.IsPending())
        {
            return Result<Unit>.Failure("already_responded", "This friend request has already been responded to");
        }

        if (request.Accept)
        {
            friendship.Accept();
        }
        else
        {
            friendship.Decline();
        }

        await _friendshipRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}