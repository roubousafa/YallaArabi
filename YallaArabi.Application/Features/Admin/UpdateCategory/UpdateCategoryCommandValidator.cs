using FluentValidation;

namespace YallaArabi.Application.Features.Admin.UpdateCategory;

public sealed class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(150).WithMessage("Category name is too long");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description is too long");

        RuleFor(x => x.IconUrl)
            .MaximumLength(500).WithMessage("Icon URL is too long");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Order index must be zero or greater");
    }
}