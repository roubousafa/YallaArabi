using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class ChallengeParticipation : AggregateRoot
{
    public Guid ChallengeId { get; private set; }
    public Guid UserId { get; private set; }
    public int TotalQuestions { get; private set; }
    public int CorrectAnswers { get; private set; }
    public int PointsEarned { get; private set; }
    public string Status { get; private set; } = default!;
    public DateTime StartedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private ChallengeParticipation() { }

    public static ChallengeParticipation Start(Guid challengeId, Guid userId, int totalQuestions)
    {
        if (totalQuestions <= 0)
            throw new ArgumentException("A challenge must have at least one question", nameof(totalQuestions));

        return new ChallengeParticipation
        {
            Id = Guid.NewGuid(),
            ChallengeId = challengeId,
            UserId = userId,
            TotalQuestions = totalQuestions,
            CorrectAnswers = 0,
            PointsEarned = 0,
            Status = "in_progress",
            StartedAt = DateTime.UtcNow,
            CompletedAt = null
        };
    }

    public void Complete(int correctAnswers, int pointsEarned)
    {
        if (correctAnswers < 0 || correctAnswers > TotalQuestions)
            throw new ArgumentException("Correct answers count is invalid", nameof(correctAnswers));

        CorrectAnswers = correctAnswers;
        PointsEarned = pointsEarned;
        Status = "completed";
        CompletedAt = DateTime.UtcNow;
    }

    public bool IsCompleted() => Status == "completed";

    public int ScorePercentage() =>
        TotalQuestions == 0 ? 0 : (int)Math.Round(CorrectAnswers * 100.0 / TotalQuestions);

    public bool IsExpired(int timeLimitSeconds) =>
        Status == "in_progress" && DateTime.UtcNow > StartedAt.AddSeconds(timeLimitSeconds);
}