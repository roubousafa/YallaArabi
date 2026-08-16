using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Friends.SendFriendRequest;

public sealed class SendFriendRequestCommandHandler : IRequestHandler<SendFriendRequestCommand, Result<SendFriendRequestResponse>>
{
    private readonly IFriendshipRepository _friendshipRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public SendFriendRequestCommandHandler(
        IFriendshipRepository friendshipRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _friendshipRepository = friendshipRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SendFriendRequestResponse>> Handle(SendFriendRequestCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<SendFriendRequestResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        if (currentUserId.Value == request.AddresseeId)
        {
            return Result<SendFriendRequestResponse>.Failure("invalid_request", "You cannot send a friend request to yourself");
        }

        var addressee = await _userRepository.GetByIdAsync(request.AddresseeId, cancellationToken);

        if (addressee is null)
        {
            return Result<SendFriendRequestResponse>.Failure("user_not_found", "User not found");
        }

        var existing = await _friendshipRepository.GetBetweenUsersAsync(currentUserId.Value, request.AddresseeId, cancellationToken);

        if (existing is not null)
        {
            if (existing.IsAccepted())
            {
                return Result<SendFriendRequestResponse>.Failure("already_friends", "You are already friends with this user");
            }

            if (existing.IsPending())
            {
                return Result<SendFriendRequestResponse>.Failure("request_already_pending", "A friend request is already pending between you and this user");
            }

            await _friendshipRepository.RemoveAsync(existing, cancellationToken);
            await _friendshipRepository.SaveChangesAsync(cancellationToken);
        }

        var friendship = Friendship.SendRequest(currentUserId.Value, request.AddresseeId);

        await _friendshipRepository.AddAsync(friendship, cancellationToken);
        await _friendshipRepository.SaveChangesAsync(cancellationToken);

        return Result<SendFriendRequestResponse>.Success(
            new SendFriendRequestResponse(friendship.Id, friendship.Status));
    }
}