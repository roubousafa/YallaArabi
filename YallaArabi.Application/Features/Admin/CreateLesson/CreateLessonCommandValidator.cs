using FluentValidation;

namespace YallaArabi.Application.Features.Admin.CreateLesson;

public sealed class CreateLessonCommandValidator : AbstractValidator<CreateLessonCommand>
{
    public CreateLessonCommandValidator()
    {
        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Lesson title is required")
            .MaximumLength(200).WithMessage("Lesson title is too long");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description is too long");

        RuleFor(x => x.IconUrl)
            .MaximumLength(500).WithMessage("Icon URL is too long");

        RuleFor(x => x.PointsReward)
            .GreaterThanOrEqualTo(0).WithMessage("Points reward cannot be negative")
            .LessThanOrEqualTo(10000).WithMessage("Points reward is too high");
    }
}