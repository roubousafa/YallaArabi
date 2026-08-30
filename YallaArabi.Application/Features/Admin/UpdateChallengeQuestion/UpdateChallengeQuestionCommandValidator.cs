using FluentValidation;

namespace YallaArabi.Application.Features.Admin.UpdateChallengeQuestion;

public sealed class UpdateChallengeQuestionCommandValidator : AbstractValidator<UpdateChallengeQuestionCommand>
{
    private static readonly string[] ValidOptions = { "A", "B", "C", "D" };

    public UpdateChallengeQuestionCommandValidator()
    {
        RuleFor(x => x.QuestionText)
            .NotEmpty().WithMessage("Question text is required")
            .MaximumLength(500).WithMessage("Question text is too long");

        RuleFor(x => x.OptionA)
            .NotEmpty().WithMessage("Option A is required")
            .MaximumLength(200).WithMessage("Option A is too long");

        RuleFor(x => x.OptionB)
            .NotEmpty().WithMessage("Option B is required")
            .MaximumLength(200).WithMessage("Option B is too long");

        RuleFor(x => x.OptionC)
            .MaximumLength(200).WithMessage("Option C is too long");

        RuleFor(x => x.OptionD)
            .MaximumLength(200).WithMessage("Option D is too long");

        RuleFor(x => x.CorrectOption)
            .NotEmpty().WithMessage("Correct option is required")
            .Must(option => ValidOptions.Contains(option?.Trim().ToUpperInvariant()))
            .WithMessage("Correct option must be A, B, C or D");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0).WithMessage("Order index must be zero or greater");
    }
}