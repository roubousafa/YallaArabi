using Mediator;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Auth.ForgotPassword;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IEmailSender _emailSender;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator,
        IEmailSender emailSender)
    {
        _userRepository = userRepository;
        _jwtTokenGenerator = jwtTokenGenerator;
        _emailSender = emailSender;
    }

    public async ValueTask<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null)
        {
            return Unit.Value;
        }

        var resetTokenPlain = _jwtTokenGenerator.GenerateRefreshToken();
        var resetTokenHash = HashToken(resetTokenPlain);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        user.SetPasswordResetToken(resetTokenHash, expiresAt);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var resetLink = $"https://yourapp.com/reset-password?token={resetTokenPlain}&email={user.Email.Value}";
        await _emailSender.SendPasswordResetEmailAsync(user.Email.Value, user.FullName, resetLink, cancellationToken);

        return Unit.Value;
    }

    private static string HashToken(string token)
    {
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        var bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}