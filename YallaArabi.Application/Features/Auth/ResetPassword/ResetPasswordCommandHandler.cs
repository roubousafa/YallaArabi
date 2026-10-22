using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Auth.ResetPassword;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public ResetPasswordCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async ValueTask<Result<Unit>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Result<Unit>.Failure("invalid_token", "Invalid or expired reset token");
        }

        var tokenHash = HashToken(request.Token);

        if (!user.IsPasswordResetTokenValid(tokenHash))
        {
            return Result<Unit>.Failure("invalid_token", "Invalid or expired reset token");
        }

        var newPasswordHash = _passwordHasher.Hash(request.NewPassword);
        user.ChangePasswordHash(newPasswordHash);
        user.ClearPasswordResetToken();
        user.ClearPasswordChangeRequirement();
        user.ClearFailedLogins();
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }

    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}