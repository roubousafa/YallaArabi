using FluentValidation;

namespace YallaArabi.Application.Features.Admin.AddLevel;

public sealed class AddLevelCommandValidator : AbstractValidator<AddLevelCommand>
{
    public AddLevelCommandValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Level code is required")
            .MaximumLength(10).WithMessage("Level code is too long");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Level name is required")
            .MaximumLength(100).WithMessage("Level name is too long");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Order index must be zero or greater");
    }
}