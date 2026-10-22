using System.Security.Cryptography;
using System.Text;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Services;

public sealed class EmailVerificationService : IEmailVerificationService
{
    private readonly IEmailSender _emailSender;

    public EmailVerificationService(IEmailSender emailSender) => _emailSender = emailSender;

    public string IssueCode(User user)
    {
        var code = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        user.SetEmailVerificationCode(HashCode(user.Id, code), DateTime.UtcNow);
        return code;
    }

    public async Task SendCodeAsync(User user, string code, CancellationToken ct = default)
    {
        try
        {
            await _emailSender.SendEmailVerificationCodeAsync(user.Email.Value, user.FullName, code, ct);
        }
        catch
        {
            // The sender logs the failure; the student can ask for a new code from the app.
        }
    }

    // Salted with the user id so the same code never hashes the same way for two people.
    public string HashCode(Guid userId, string code)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{userId:N}:{code.Trim()}"));
        return Convert.ToBase64String(bytes);
    }
}
