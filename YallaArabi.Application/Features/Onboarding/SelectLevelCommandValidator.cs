using FluentValidation;

namespace YallaArabi.Application.Features.Onboarding;

public sealed class SelectLevelCommandValidator : AbstractValidator<SelectLevelCommand>
{
    public SelectLevelCommandValidator()
    {
        RuleFor(x => x.AssessmentKey)
            .NotEmpty().WithMessage("Assessment key is required")
            .Must(key => SelfAssessmentOptions.All.Any(o => o.Key == key))
            .WithMessage("Invalid assessment key");
    }
}