using FluentValidation;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Admin.AddLessonQuestion;

public interface ILessonQuestionFields
{
    string Type { get; }
    string? QuestionText { get; }
    string? OptionA { get; }
    string? OptionB { get; }
    string? OptionC { get; }
    string? OptionD { get; }
    string? CorrectOption { get; }
    Guid? WordId { get; }
    string? AudioUrl { get; }
    string? ImageUrl { get; }
}

// Shared by add and update so both enforce the same per-type rules as LessonQuestion.
public abstract class LessonQuestionFieldsValidator<T> : AbstractValidator<T> where T : ILessonQuestionFields
{
    private static readonly string[] ValidOptions = { "A", "B", "C", "D" };

    protected LessonQuestionFieldsValidator()
    {
        RuleFor(x => x.Type)
            .Must(LessonQuestionTypes.IsSupported)
            .WithMessage($"Type must be one of: {string.Join(", ", LessonQuestionTypes.All)}");

        RuleFor(x => x.QuestionText).MaximumLength(500).WithMessage("Question text is too long");
        RuleFor(x => x.OptionA).MaximumLength(200).WithMessage("Option A is too long");
        RuleFor(x => x.OptionB).MaximumLength(200).WithMessage("Option B is too long");
        RuleFor(x => x.OptionC).MaximumLength(200).WithMessage("Option C is too long");
        RuleFor(x => x.OptionD).MaximumLength(200).WithMessage("Option D is too long");
        RuleFor(x => x.AudioUrl).MaximumLength(500).WithMessage("Audio URL is too long");
        RuleFor(x => x.ImageUrl).MaximumLength(500).WithMessage("Image URL is too long");

        When(x => Normalize(x.Type) == LessonQuestionTypes.MultipleChoice, () =>
        {
            RuleFor(x => x.QuestionText).NotEmpty().WithMessage("Question text is required for multiple choice questions");
        });

        When(x => Normalize(x.Type) == LessonQuestionTypes.ListenChoose, () =>
        {
            RuleFor(x => x)
                .Must(x => x.WordId.HasValue || !string.IsNullOrWhiteSpace(x.AudioUrl))
                .WithName("AudioUrl")
                .WithMessage("A listening question needs a wordId or an audioUrl");
        });

        When(x => Normalize(x.Type) == LessonQuestionTypes.SpeakRepeat, () =>
        {
            RuleFor(x => x.WordId).NotNull().WithMessage("A speaking question needs a wordId");
        });

        When(x => LessonQuestionTypes.IsSupported(x.Type) && LessonQuestionTypes.HasOptions(Normalize(x.Type)), () =>
        {
            RuleFor(x => x.OptionA).NotEmpty().WithMessage("Option A is required");
            RuleFor(x => x.OptionB).NotEmpty().WithMessage("Option B is required");

            RuleFor(x => x.CorrectOption)
                .Must(option => ValidOptions.Contains(option?.Trim().ToUpperInvariant()))
                .WithMessage("Correct option must be A, B, C or D");

            RuleFor(x => x)
                .Must(x => !(Normalize(x.CorrectOption) == "c" && string.IsNullOrWhiteSpace(x.OptionC))
                        && !(Normalize(x.CorrectOption) == "d" && string.IsNullOrWhiteSpace(x.OptionD)))
                .WithName("CorrectOption")
                .WithMessage("The correct option cannot point to an empty option");
        });
    }

    private static string Normalize(string? value) => value?.Trim().ToLowerInvariant() ?? string.Empty;
}
