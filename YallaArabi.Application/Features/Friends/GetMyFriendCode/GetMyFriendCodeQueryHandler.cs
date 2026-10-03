using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Friends.GetMyFriendCode;

public sealed class GetMyFriendCodeQueryHandler : IRequestHandler<GetMyFriendCodeQuery, Result<MyFriendCodeResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IFriendCodeService _friendCodeService;
    private readonly ICurrentUserService _currentUserService;

    public GetMyFriendCodeQueryHandler(
        IUserRepository userRepository,
        IFriendCodeService friendCodeService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _friendCodeService = friendCodeService;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<MyFriendCodeResponse>> Handle(GetMyFriendCodeQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<MyFriendCodeResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<MyFriendCodeResponse>.Failure("user_not_found", "User not found");
        }

        if (string.IsNullOrWhiteSpace(user.FriendCode))
        {
            var code = await _friendCodeService.GenerateUniqueAsync(cancellationToken);
            user.SetFriendCode(code);
            await _userRepository.SaveChangesAsync(cancellationToken);
        }

        return Result<MyFriendCodeResponse>.Success(
            new MyFriendCodeResponse(user.Id, user.FullName, user.FriendCode!));
    }
}