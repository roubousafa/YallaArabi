using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class ChallengeAnswer : Entity
{
    public Guid ParticipationId { get; private set; }
    public Guid QuestionId { get; private set; }
    public string? SelectedOption { get; private set; }
    public string CorrectOption { get; private set; } = default!;
    public bool IsCorrect { get; private set; }
    public int OrderIndex { get; private set; }

    private ChallengeAnswer() { }

    public static ChallengeAnswer Create(
        Guid participationId,
        Guid questionId,
        string? selectedOption,
        string correctOption,
        bool isCorrect,
        int orderIndex)
    {
        return new ChallengeAnswer
        {
            Id = Guid.NewGuid(),
            ParticipationId = participationId,
            QuestionId = questionId,
            SelectedOption = selectedOption,
            CorrectOption = correctOption,
            IsCorrect = isCorrect,
            OrderIndex = orderIndex
        };
    }
}