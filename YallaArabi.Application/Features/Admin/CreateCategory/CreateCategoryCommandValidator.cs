using FluentValidation;

namespace YallaArabi.Application.Features.Admin.CreateCategory;

public sealed class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.LevelCode)
            .NotEmpty().WithMessage("Level code is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Category name is required")
            .MaximumLength(150).WithMessage("Category name is too long");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description is too long");

        RuleFor(x => x.IconUrl)
            .MaximumLength(500).WithMessage("Icon URL is too long");
    }
}