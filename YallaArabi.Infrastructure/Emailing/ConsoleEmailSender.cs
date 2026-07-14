using Microsoft.Extensions.Logging;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Emailing;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger) => _logger = logger;

    public Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "[DEV EMAIL] Password reset link for {FullName} <{Email}>: {ResetLink}",
            fullName, toEmail, resetLink);

        return Task.CompletedTask;
    }
}