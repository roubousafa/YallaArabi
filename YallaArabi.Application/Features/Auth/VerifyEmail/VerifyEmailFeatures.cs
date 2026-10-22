using FluentValidation;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Auth.VerifyEmail;

// AccessToken is a fresh token that already says the email is verified; null when nothing changed.
public sealed record EmailVerificationResponse(bool IsEmailVerified, string? AccessToken = null);

public sealed record VerifyEmailCommand(string Code) : IRequest<Result<EmailVerificationResponse>>;

public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required")
            .Matches("^[0-9]{6}$").WithMessage("The code is 6 digits");
    }
}

public sealed class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result<EmailVerificationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationService _verificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRoleRepository _roleRepository;

    public VerifyEmailCommandHandler(
        IUserRepository userRepository,
        IEmailVerificationService verificationService,
        ICurrentUserService currentUserService,
        IJwtTokenGenerator jwtTokenGenerator,
        IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _verificationService = verificationService;
        _currentUserService = currentUserService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _roleRepository = roleRepository;
    }

    public async ValueTask<Result<EmailVerificationResponse>> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<EmailVerificationResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<EmailVerificationResponse>.Failure("user_not_found", "User not found");
        }

        var result = user.VerifyEmailCode(_verificationService.HashCode(user.Id, request.Code), DateTime.UtcNow);
        await _userRepository.SaveChangesAsync(cancellationToken);

        if (result == EmailVerificationResult.Verified)
        {
            var permissions = user.RoleId.HasValue
                ? await _roleRepository.GetPermissionKeysAsync(user.RoleId.Value, cancellationToken)
                : null;

            return Result<EmailVerificationResponse>.Success(
                new EmailVerificationResponse(true, _jwtTokenGenerator.GenerateAccessToken(user, permissions)));
        }

        return result switch
        {
            EmailVerificationResult.Invalid => Result<EmailVerificationResponse>.Failure("invalid_code", "The code is not correct"),
            EmailVerificationResult.TooManyAttempts => Result<EmailVerificationResponse>.Failure(
                "too_many_attempts", "Too many wrong codes. Ask for a new code."),
            _ => Result<EmailVerificationResponse>.Failure("code_expired", "The code has expired. Ask for a new code.")
        };
    }
}

public sealed record ResendEmailVerificationCommand : IRequest<Result<EmailVerificationResponse>>;

public sealed class ResendEmailVerificationCommandHandler
    : IRequestHandler<ResendEmailVerificationCommand, Result<EmailVerificationResponse>>
{
    private static readonly TimeSpan ResendCooldown = TimeSpan.FromSeconds(60);

    private readonly IUserRepository _userRepository;
    private readonly IEmailVerificationService _verificationService;
    private readonly ICurrentUserService _currentUserService;

    public ResendEmailVerificationCommandHandler(
        IUserRepository userRepository,
        IEmailVerificationService verificationService,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _verificationService = verificationService;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<EmailVerificationResponse>> Handle(ResendEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<EmailVerificationResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<EmailVerificationResponse>.Failure("user_not_found", "User not found");
        }

        if (user.IsEmailVerified)
        {
            return Result<EmailVerificationResponse>.Failure("already_verified", "This email is already verified");
        }

        if (user.EmailVerificationSentAt is { } sentAt && DateTime.UtcNow - sentAt < ResendCooldown)
        {
            return Result<EmailVerificationResponse>.Failure("resend_too_soon", "Please wait a minute before asking for another code");
        }

        var code = _verificationService.IssueCode(user);
        await _userRepository.SaveChangesAsync(cancellationToken);
        await _verificationService.SendCodeAsync(user, code, cancellationToken);

        return Result<EmailVerificationResponse>.Success(new EmailVerificationResponse(false));
    }
}
