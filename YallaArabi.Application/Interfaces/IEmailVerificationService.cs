using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IEmailVerificationService
{
    // Puts a fresh code on the user. Save the user, then call SendCodeAsync with the returned code.
    string IssueCode(User user);

    // Sends the code; a failed send is logged and swallowed so it never undoes the caller's work.
    Task SendCodeAsync(User user, string code, CancellationToken ct = default);

    string HashCode(Guid userId, string code);
}
