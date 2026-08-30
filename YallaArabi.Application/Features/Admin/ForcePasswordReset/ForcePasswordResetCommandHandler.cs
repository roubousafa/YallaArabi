using System.Security.Cryptography;
using System.Text;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.ForcePasswordReset;

public sealed class ForcePasswordResetCommandHandler : IRequestHandler<ForcePasswordResetCommand, Result<ForcePasswordResetResponse>>
{
    private const int ResetTokenValidMinutes = 60;

    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IEmailSender _emailSender;
    private readonly ICurrentUserService _currentUserService;

    public ForcePasswordResetCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IEmailSender emailSender,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _emailSender = emailSender;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<ForcePasswordResetResponse>> Handle(ForcePasswordResetCommand request, CancellationToken cancellationToken)
    {
        var currentAdminId = _currentUserService.UserId;

        if (currentAdminId is null)
        {
            return Result<ForcePasswordResetResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var processedCount = 0;
        var sessionsRevoked = 0;
        var emailsSent = 0;
        var skipped = new List<SkippedUser>();

        foreach (var userId in request.UserIds.Distinct())
        {
            if (userId == currentAdminId.Value)
            {
                skipped.Add(new SkippedUser(userId, "You cannot force a password reset on your own account"));
                continue;
            }

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user is null)
            {
                skipped.Add(new SkippedUser(userId, "User not found"));
                continue;
            }

            if (request.RevokeSessions)
            {
                var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(userId, cancellationToken);

                foreach (var token in activeTokens)
                {
                    token.Revoke();
                    sessionsRevoked++;
                }

                await _refreshTokenRepository.SaveChangesAsync(cancellationToken);
            }

            if (user.PasswordHash is null)
            {
                skipped.Add(new SkippedUser(userId, "Google account without a password. Sessions revoked only."));
                processedCount++;
                continue;
            }

            user.RequirePasswordChange();

            if (request.SendResetEmail)
            {
                var resetTokenPlain = GenerateResetToken();
                var resetTokenHash = HashToken(resetTokenPlain);

                user.SetPasswordResetToken(resetTokenHash, DateTime.UtcNow.AddMinutes(ResetTokenValidMinutes));
                await _userRepository.SaveChangesAsync(cancellationToken);

                var resetLink = $"https://yourapp.com/reset-password?token={resetTokenPlain}&email={user.Email.Value}";

                try
                {
                    await _emailSender.SendPasswordResetEmailAsync(user.Email.Value, user.FullName, resetLink, cancellationToken);
                    emailsSent++;
                }
                catch
                {
                    skipped.Add(new SkippedUser(userId, "Password reset enforced, but the email could not be sent"));
                }
            }
            else
            {
                await _userRepository.SaveChangesAsync(cancellationToken);
            }

            processedCount++;
        }

        return Result<ForcePasswordResetResponse>.Success(
            new ForcePasswordResetResponse(processedCount, sessionsRevoked, emailsSent, skipped));
    }

    private static string GenerateResetToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(bytes);
    }

    private static string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(bytes);
    }
}