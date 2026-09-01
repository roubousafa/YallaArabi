using FluentValidation;

namespace YallaArabi.Application.Features.Admin.CreateRole;

public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleCommandValidator()
    {
        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("English name is required")
            .MaximumLength(100).WithMessage("English name is too long");

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Arabic name is required")
            .MaximumLength(100).WithMessage("Arabic name is too long");

        RuleFor(x => x.Description)
            .MaximumLength(300).WithMessage("Description is too long");

        RuleFor(x => x.Permissions)
            .NotNull().WithMessage("Permissions list is required");
    }
}