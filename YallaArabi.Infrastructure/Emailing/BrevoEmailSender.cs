using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Emailing;

public class BrevoEmailSender : IEmailSender
{
    private readonly EmailOptions _options;
    private readonly ILogger<BrevoEmailSender> _logger;

    public BrevoEmailSender(IOptions<EmailOptions> options, ILogger<BrevoEmailSender> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        message.To.Add(new MailboxAddress(fullName, toEmail));
        message.Subject = "Reset your Yalla Arabi password";

        message.Body = new TextPart("plain")
        {
            Text = $"Hi {fullName},\n\n" +
                   $"We received a request to reset your password. Click the link below to choose a new one:\n\n" +
                   $"{resetLink}\n\n" +
                   $"This link will expire in 15 minutes. If you didn't request this, you can safely ignore this email.\n\n" +
                   $"— Yalla Arabi Team"
        };

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_options.SmtpServer, _options.SmtpPort, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_options.SmtpLogin, _options.SmtpKey, ct);
            await client.SendAsync(message, ct);

            _logger.LogInformation("Password reset email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send password reset email to {Email}", toEmail);
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }
}