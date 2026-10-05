using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class LessonQuestion : Entity
{
    public Guid LessonId { get; private set; }
    public string Type { get; private set; } = default!;
    public string? QuestionText { get; private set; }
    public string? OptionA { get; private set; }
    public string? OptionB { get; private set; }
    public string? OptionC { get; private set; }
    public string? OptionD { get; private set; }
    public string? CorrectOption { get; private set; }
    public Guid? WordId { get; private set; }
    public string? AudioUrl { get; private set; }
    public string? ImageUrl { get; private set; }
    public int OrderIndex { get; private set; }

    private LessonQuestion() { }

    public static LessonQuestion Create(
        Guid lessonId,
        string type,
        string? questionText,
        string? optionA,
        string? optionB,
        string? optionC,
        string? optionD,
        string? correctOption,
        Guid? wordId,
        string? audioUrl,
        string? imageUrl,
        int orderIndex)
    {
        var question = new LessonQuestion { Id = Guid.NewGuid(), LessonId = lessonId };
        question.Apply(type, questionText, optionA, optionB, optionC, optionD, correctOption, wordId, audioUrl, imageUrl, orderIndex);
        return question;
    }

    public void Update(
        string type,
        string? questionText,
        string? optionA,
        string? optionB,
        string? optionC,
        string? optionD,
        string? correctOption,
        Guid? wordId,
        string? audioUrl,
        string? imageUrl,
        int orderIndex)
    {
        Apply(type, questionText, optionA, optionB, optionC, optionD, correctOption, wordId, audioUrl, imageUrl, orderIndex);
    }

    private void Apply(
        string type,
        string? questionText,
        string? optionA,
        string? optionB,
        string? optionC,
        string? optionD,
        string? correctOption,
        Guid? wordId,
        string? audioUrl,
        string? imageUrl,
        int orderIndex)
    {
        if (!LessonQuestionTypes.IsSupported(type))
            throw new ArgumentException("Unsupported question type", nameof(type));

        var normalizedType = type.Trim().ToLowerInvariant();
        var hasOptions = LessonQuestionTypes.HasOptions(normalizedType);

        if (normalizedType == LessonQuestionTypes.MultipleChoice && string.IsNullOrWhiteSpace(questionText))
            throw new ArgumentException("Question text is required for multiple choice questions", nameof(questionText));

        if (normalizedType == LessonQuestionTypes.ListenChoose && wordId is null && string.IsNullOrWhiteSpace(audioUrl))
            throw new ArgumentException("A listening question needs a word or an audio URL", nameof(audioUrl));

        if (normalizedType == LessonQuestionTypes.SpeakRepeat && wordId is null)
            throw new ArgumentException("A speaking question needs a word", nameof(wordId));

        string? normalizedCorrect = null;

        if (hasOptions)
        {
            if (string.IsNullOrWhiteSpace(optionA) || string.IsNullOrWhiteSpace(optionB))
                throw new ArgumentException("Options A and B are required", nameof(optionA));

            normalizedCorrect = correctOption?.Trim().ToUpperInvariant();

            if (normalizedCorrect is not ("A" or "B" or "C" or "D"))
                throw new ArgumentException("Correct option must be A, B, C or D", nameof(correctOption));

            if (normalizedCorrect == "C" && string.IsNullOrWhiteSpace(optionC))
                throw new ArgumentException("Option C cannot be the correct answer when it is empty", nameof(correctOption));

            if (normalizedCorrect == "D" && string.IsNullOrWhiteSpace(optionD))
                throw new ArgumentException("Option D cannot be the correct answer when it is empty", nameof(correctOption));
        }

        Type = normalizedType;
        QuestionText = string.IsNullOrWhiteSpace(questionText) ? null : questionText.Trim();
        OptionA = hasOptions ? optionA!.Trim() : null;
        OptionB = hasOptions ? optionB!.Trim() : null;
        OptionC = hasOptions && !string.IsNullOrWhiteSpace(optionC) ? optionC.Trim() : null;
        OptionD = hasOptions && !string.IsNullOrWhiteSpace(optionD) ? optionD.Trim() : null;
        CorrectOption = normalizedCorrect;
        WordId = wordId;
        AudioUrl = string.IsNullOrWhiteSpace(audioUrl) ? null : audioUrl.Trim();
        ImageUrl = string.IsNullOrWhiteSpace(imageUrl) ? null : imageUrl.Trim();
        OrderIndex = orderIndex;
    }
}
