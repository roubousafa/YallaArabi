using FluentValidation;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Auth.ChangeEmail;

public sealed record ChangeEmailCommand(string NewEmail, string CurrentPassword) : IRequest<Result<ChangeEmailResponse>>;

public sealed record ChangeEmailResponse(string Email);

public sealed class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
{
    public ChangeEmailCommandValidator()
    {
        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("New email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required");
    }
}

public sealed class ChangeEmailCommandHandler : IRequestHandler<ChangeEmailCommand, Result<ChangeEmailResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailSender _emailSender;
    private readonly ICurrentUserService _currentUserService;

    public ChangeEmailCommandHandler(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IPasswordHasher passwordHasher,
        IEmailSender emailSender,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _passwordHasher = passwordHasher;
        _emailSender = emailSender;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<ChangeEmailResponse>> Handle(ChangeEmailCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<ChangeEmailResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<ChangeEmailResponse>.Failure("user_not_found", "User not found");
        }

        // Google-only accounts have no password to confirm the change, and their email belongs to Google.
        if (user.PasswordHash is null)
        {
            return Result<ChangeEmailResponse>.Failure(
                "no_password_set",
                "This account signs in with Google, so its email is managed by Google.");
        }

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return Result<ChangeEmailResponse>.Failure("invalid_password", "Current password is incorrect");
        }

        var oldEmail = user.Email.Value;
        var newEmail = request.NewEmail.Trim();

        if (string.Equals(oldEmail, newEmail, StringComparison.OrdinalIgnoreCase))
        {
            return Result<ChangeEmailResponse>.Failure("same_email", "The new email is the same as the current one");
        }

        if (await _userRepository.EmailExistsAsync(newEmail, cancellationToken))
        {
            return Result<ChangeEmailResponse>.Failure("email_already_exists", "This email is already registered");
        }

        user.ChangeEmail(newEmail);

        // Access tokens carry the email claim, so every session signs in again with the new address.
        var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);

        foreach (var token in activeTokens)
        {
            token.Revoke();
        }

        await _userRepository.SaveChangesAsync(cancellationToken);

        try
        {
            await _emailSender.SendEmailChangedNoticeAsync(oldEmail, user.FullName, user.Email.Value, cancellationToken);
        }
        catch
        {
            // The change is already saved; a failed notice (logged by the sender) must not undo it.
        }

        return Result<ChangeEmailResponse>.Success(new ChangeEmailResponse(user.Email.Value));
    }
}
