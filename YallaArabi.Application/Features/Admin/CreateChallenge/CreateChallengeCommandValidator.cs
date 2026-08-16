using FluentValidation;

namespace YallaArabi.Application.Features.Admin.CreateChallenge;

public sealed class CreateChallengeCommandValidator : AbstractValidator<CreateChallengeCommand>
{
    public CreateChallengeCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Challenge title is required")
            .MaximumLength(200).WithMessage("Challenge title is too long");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description is too long");

        RuleFor(x => x.PointsReward)
            .GreaterThanOrEqualTo(0).WithMessage("Points reward cannot be negative")
            .LessThanOrEqualTo(10000).WithMessage("Points reward is too high");

        RuleFor(x => x.TimeLimitSeconds)
            .GreaterThan(0).WithMessage("Time limit must be greater than zero")
            .LessThanOrEqualTo(3600).WithMessage("Time limit cannot exceed one hour");
    }
}