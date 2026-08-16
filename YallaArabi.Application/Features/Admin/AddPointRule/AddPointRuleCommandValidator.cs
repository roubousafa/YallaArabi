using FluentValidation;

namespace YallaArabi.Application.Features.Admin.AddPointRule;

public sealed class AddPointRuleCommandValidator : AbstractValidator<AddPointRuleCommand>
{
    public AddPointRuleCommandValidator()
    {
        RuleFor(x => x.ActionKey)
            .NotEmpty().WithMessage("Action key is required")
            .MaximumLength(50).WithMessage("Action key is too long")
            .Matches("^[a-z0-9_]+$").WithMessage("Action key can only contain lowercase letters, numbers and underscores");

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