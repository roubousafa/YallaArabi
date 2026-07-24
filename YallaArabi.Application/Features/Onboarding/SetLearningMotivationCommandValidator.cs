using FluentValidation;

namespace YallaArabi.Application.Features.Onboarding;

public sealed class SetLearningMotivationCommandValidator : AbstractValidator<SetLearningMotivationCommand>
{
    public SetLearningMotivationCommandValidator()
    {
        RuleFor(x => x.MotivationKey)
            .NotEmpty().WithMessage("Motivation key is required")
            .Must(key => LearningMotivationOptions.All.Any(o => o.Key == key))
            .WithMessage("Invalid motivation key");
    }
}