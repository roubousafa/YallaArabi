using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Friends.RespondToFriendRequest;

public sealed record RespondToFriendRequestCommand(Guid FriendshipId, bool Accept) : IRequest<Result<Unit>>;