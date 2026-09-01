using FluentValidation;

namespace YallaArabi.Application.Features.Admin.UpdateTeamMember;

public sealed class UpdateTeamMemberCommandValidator : AbstractValidator<UpdateTeamMemberCommand>
{
    public UpdateTeamMemberCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(150).WithMessage("Full name is too long");

        RuleFor(x => x.RoleId)
            .NotEmpty().WithMessage("Role is required for team members");
    }
}