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

    public async Task SendEmailChangedNoticeAsync(string oldEmail, string fullName, string newEmail, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        message.To.Add(new MailboxAddress(fullName, oldEmail));
        message.Subject = "Your Yalla Arabi email was changed";
        message.Body = new TextPart("plain")
        {
            Text = $"Hi {fullName},\n\n" +
                   $"The email address on your Yalla Arabi account was changed to {newEmail}.\n\n" +
                   $"If you made this change, no action is needed. If you didn't, please contact support immediately.\n\n" +
                   $"— Yalla Arabi Team"
        };

        await SendAsync(message, oldEmail, "Email change notice", ct);
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

        await SendAsync(message, toEmail, "Password reset email", ct);
    }

    public async Task SendEmailVerificationCodeAsync(string toEmail, string fullName, string code, CancellationToken ct = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.SenderName, _options.SenderEmail));
        message.To.Add(new MailboxAddress(fullName, toEmail));
        message.Subject = $"{code} is your Yalla Arabi code / رمز التأكيد";
        message.Body = new TextPart("plain")
        {
            Text = $"مرحبا {fullName}،\n\n" +
                   $"رمز تأكيد إيميلك بتطبيق يلا عربي هو: {code}\n" +
                   $"الرمز صالح لمدة 30 دقيقة. إذا ما سجّلت حساب، تجاهل هالرسالة.\n\n" +
                   $"Hi {fullName},\n\n" +
                   $"Your Yalla Arabi verification code is: {code}\n" +
                   $"It expires in 30 minutes. If you didn't create an account, you can ignore this email.\n\n" +
                   $"— Yalla Arabi Team"
        };

        await SendAsync(message, toEmail, "Email verification code", ct);
    }

    private async Task SendAsync(MimeMessage message, string toEmail, string emailKind, CancellationToken ct)
    {
        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_options.SmtpServer, _options.SmtpPort, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_options.SmtpLogin, _options.SmtpKey, ct);
            await client.SendAsync(message, ct);

            _logger.LogInformation("{EmailKind} sent successfully to {Email}", emailKind, toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send {EmailKind} to {Email}", emailKind, toEmail);
            throw;
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }
}