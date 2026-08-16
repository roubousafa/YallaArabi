using FluentValidation;

namespace YallaArabi.Application.Features.Admin.UpdatePointRule;

public sealed class UpdatePointRuleCommandValidator : AbstractValidator<UpdatePointRuleCommand>
{
    public UpdatePointRuleCommandValidator()
    {
        RuleFor(x => x.DisplayName)
            .NotEmpty().WithMessage("Display name is required")
            .MaximumLength(150).WithMessage("Display name is too long");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description is too long");

        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0).WithMessage("Points cannot be negative")
            .LessThanOrEqualTo(10000).WithMessage("Points value is too high");
    }
}