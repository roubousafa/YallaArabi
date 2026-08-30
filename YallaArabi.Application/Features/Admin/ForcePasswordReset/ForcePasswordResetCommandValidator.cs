using FluentValidation;

namespace YallaArabi.Application.Features.Admin.ForcePasswordReset;

public sealed class ForcePasswordResetCommandValidator : AbstractValidator<ForcePasswordResetCommand>
{
    public ForcePasswordResetCommandValidator()
    {
        RuleFor(x => x.UserIds)
            .NotEmpty().WithMessage("At least one user must be selected")
            .Must(ids => ids.Count <= 200).WithMessage("Cannot process more than 200 users at once");
    }
}