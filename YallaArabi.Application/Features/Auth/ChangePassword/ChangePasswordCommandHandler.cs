using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Auth.ChangePassword;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUserService;

    public ChangePasswordCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
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

        if (user.PasswordHash is null)
        {
            return Result<Unit>.Failure(
                "no_password_set",
                "This account signs in with Google and has no password. Use forgot password to set one.");
        }

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return Result<Unit>.Failure("invalid_password", "Current password is incorrect");
        }

        var newHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePasswordHash(newHash);
        user.ClearPasswordChangeRequirement();

        var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke();
        }

        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}