using FluentValidation;

namespace YallaArabi.Application.Features.Admin.UpdateLevel;

public sealed class UpdateLevelCommandValidator : AbstractValidator<UpdateLevelCommand>
{
    public UpdateLevelCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Level name is required")
            .MaximumLength(100).WithMessage("Level name is too long");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Order index must be zero or greater");
    }
}