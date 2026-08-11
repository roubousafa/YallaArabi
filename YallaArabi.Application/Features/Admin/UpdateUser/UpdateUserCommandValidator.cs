using FluentValidation;

namespace YallaArabi.Application.Features.Admin.UpdateUser;

public sealed class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private static readonly string[] ValidRoles = { "Student", "Admin" };

    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(150).WithMessage("Full name is too long");

        RuleFor(x => x.Role)
            .NotEmpty().WithMessage("Role is required")
            .Must(role => ValidRoles.Contains(role))
            .WithMessage("Invalid role. Valid options: Student, Admin");
    }
}