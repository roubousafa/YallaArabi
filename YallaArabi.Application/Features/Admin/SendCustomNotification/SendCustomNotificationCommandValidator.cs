using FluentValidation;

namespace YallaArabi.Application.Features.Admin.SendCustomNotification;

public sealed class SendCustomNotificationCommandValidator : AbstractValidator<SendCustomNotificationCommand>
{
    public SendCustomNotificationCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Notification title is required")
            .MaximumLength(200).WithMessage("Notification title is too long");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Notification body is required")
            .MaximumLength(1000).WithMessage("Notification body is too long");
    }
}