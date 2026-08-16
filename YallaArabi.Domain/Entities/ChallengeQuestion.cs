using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class ChallengeQuestion : Entity
{
    public Guid ChallengeId { get; private set; }
    public string QuestionText { get; private set; } = default!;
    public string OptionA { get; private set; } = default!;
    public string OptionB { get; private set; } = default!;
    public string? OptionC { get; private set; }
    public string? OptionD { get; private set; }
    public string CorrectOption { get; private set; } = default!;
    public int OrderIndex { get; private set; }

    private ChallengeQuestion() { }

    public static ChallengeQuestion Create(
        Guid challengeId,
        string questionText,
        string optionA,
        string optionB,
        string? optionC,
        string? optionD,
        string correctOption,
        int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(questionText))
            throw new ArgumentException("Question text is required", nameof(questionText));

        if (string.IsNullOrWhiteSpace(optionA))
            throw new ArgumentException("Option A is required", nameof(optionA));

        if (string.IsNullOrWhiteSpace(optionB))
            throw new ArgumentException("Option B is required", nameof(optionB));

        var normalizedCorrect = correctOption?.Trim().ToUpperInvariant();

        if (normalizedCorrect is not ("A" or "B" or "C" or "D"))
            throw new ArgumentException("Correct option must be A, B, C or D", nameof(correctOption));

        if (normalizedCorrect == "C" && string.IsNullOrWhiteSpace(optionC))
            throw new ArgumentException("Option C cannot be the correct answer when it is empty", nameof(correctOption));

        if (normalizedCorrect == "D" && string.IsNullOrWhiteSpace(optionD))
            throw new ArgumentException("Option D cannot be the correct answer when it is empty", nameof(correctOption));

        return new ChallengeQuestion
        {
            Id = Guid.NewGuid(),
            ChallengeId = challengeId,
            QuestionText = questionText.Trim(),
            OptionA = optionA.Trim(),
            OptionB = optionB.Trim(),
            OptionC = optionC?.Trim(),
            OptionD = optionD?.Trim(),
            CorrectOption = normalizedCorrect,
            OrderIndex = orderIndex
        };
    }

    public void Update(
        string questionText,
        string optionA,
        string optionB,
        string? optionC,
        string? optionD,
        string correctOption,
        int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(questionText))
            throw new ArgumentException("Question text is required", nameof(questionText));

        if (string.IsNullOrWhiteSpace(optionA))
            throw new ArgumentException("Option A is required", nameof(optionA));

        if (string.IsNullOrWhiteSpace(optionB))
            throw new ArgumentException("Option B is required", nameof(optionB));

        var normalizedCorrect = correctOption?.Trim().ToUpperInvariant();

        if (normalizedCorrect is not ("A" or "B" or "C" or "D"))
            throw new ArgumentException("Correct option must be A, B, C or D", nameof(correctOption));

        QuestionText = questionText.Trim();
        OptionA = optionA.Trim();
        OptionB = optionB.Trim();
        OptionC = optionC?.Trim();
        OptionD = optionD?.Trim();
        CorrectOption = normalizedCorrect;
        OrderIndex = orderIndex;
    }

    public bool IsCorrectAnswer(string selectedOption) =>
        !string.IsNullOrWhiteSpace(selectedOption)
        && selectedOption.Trim().ToUpperInvariant() == CorrectOption;
}