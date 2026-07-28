using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Notifications.RegisterFcmToken;

public sealed class RegisterFcmTokenCommandHandler : IRequestHandler<RegisterFcmTokenCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public RegisterFcmTokenCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<Unit>> Handle(RegisterFcmTokenCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<Unit>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<Unit>.Failure("user_not_found", "User not found");
        }

        user.SetFcmToken(request.FcmToken);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}