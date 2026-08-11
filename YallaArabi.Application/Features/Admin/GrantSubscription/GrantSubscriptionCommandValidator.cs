using FluentValidation;

namespace YallaArabi.Application.Features.Admin.GrantSubscription;

public sealed class GrantSubscriptionCommandValidator : AbstractValidator<GrantSubscriptionCommand>
{
    public GrantSubscriptionCommandValidator()
    {
        RuleFor(x => x.PlanCode)
            .NotEmpty().WithMessage("Plan code is required")
            .MaximumLength(50).WithMessage("Plan code is too long");

        RuleFor(x => x.DurationDays)
            .GreaterThan(0).WithMessage("Duration must be at least 1 day")
            .LessThanOrEqualTo(3650).WithMessage("Duration cannot exceed 10 years");
    }
}