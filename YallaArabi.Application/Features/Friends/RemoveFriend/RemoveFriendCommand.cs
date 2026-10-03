using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Friends.RemoveFriend;

public sealed record RemoveFriendCommand(Guid FriendshipId) : IRequest<Result<Unit>>;