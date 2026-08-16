using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Friends.SendFriendRequest;

public sealed record SendFriendRequestCommand(Guid AddresseeId) : IRequest<Result<SendFriendRequestResponse>>;

public sealed record SendFriendRequestResponse(Guid FriendshipId, string Status);