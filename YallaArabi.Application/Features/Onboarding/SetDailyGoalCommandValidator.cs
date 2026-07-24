using FluentValidation;

namespace YallaArabi.Application.Features.Onboarding;

public sealed class SetDailyGoalCommandValidator : AbstractValidator<SetDailyGoalCommand>
{
    public SetDailyGoalCommandValidator()
    {
        RuleFor(x => x.GoalKey)
            .NotEmpty().WithMessage("Goal key is required")
            .Must(key => DailyGoalOptions.All.Any(o => o.Key == key))
            .WithMessage("Invalid goal key");
    }
}