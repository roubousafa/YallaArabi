using FluentValidation;

namespace YallaArabi.Application.Features.Notifications.RegisterFcmToken;

public sealed class RegisterFcmTokenCommandValidator : AbstractValidator<RegisterFcmTokenCommand>
{
    public RegisterFcmTokenCommandValidator()
    {
        RuleFor(x => x.FcmToken)
            .NotEmpty().WithMessage("FCM token is required");
    }
}