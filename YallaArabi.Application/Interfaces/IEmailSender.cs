namespace YallaArabi.Application.Interfaces;

public interface IEmailSender
{
    Task SendPasswordResetEmailAsync(string toEmail, string fullName, string resetLink, CancellationToken ct = default);
    Task SendEmailChangedNoticeAsync(string oldEmail, string fullName, string newEmail, CancellationToken ct = default);
}