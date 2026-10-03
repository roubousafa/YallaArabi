using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Friends.GetMyFriendCode;

public sealed record GetMyFriendCodeQuery : IRequest<Result<MyFriendCodeResponse>>;

public sealed record MyFriendCodeResponse(
    Guid UserId,
    string FullName,
    string FriendCode
);