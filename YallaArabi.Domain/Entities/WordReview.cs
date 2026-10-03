using YallaArabi.Domain.Common;
using YallaArabi.Domain.Services;

namespace YallaArabi.Domain.Entities;

public sealed class WordReview : AggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid WordId { get; private set; }
    public int RepetitionCount { get; private set; }
    public double EaseFactor { get; private set; }
    public int IntervalDays { get; private set; }
    public DateTime NextReviewAt { get; private set; }
    public DateTime? LastReviewedAt { get; private set; }

    private WordReview() { }

    public static WordReview StartReviewing(Guid userId, Guid wordId)
    {
        return new WordReview
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WordId = wordId,
            RepetitionCount = 0,
            EaseFactor = 2.5,
            IntervalDays = 0,
            NextReviewAt = DateTime.UtcNow,
            LastReviewedAt = null
        };
    }

    public void ApplyReview(ReviewGrade grade)
    {
        var (newInterval, newEase, newRepetition) = SrsCalculator.CalculateNext(
            grade, RepetitionCount, EaseFactor, IntervalDays);

        RepetitionCount = newRepetition;
        EaseFactor = newEase;
        IntervalDays = newInterval;
        NextReviewAt = DateTime.UtcNow.AddDays(newInterval);
        LastReviewedAt = DateTime.UtcNow;
    }

    public bool IsDue() => NextReviewAt <= DateTime.UtcNow;
}

public enum ReviewGrade
{
    Hard = 0,
    Medium = 1,
    Easy = 2
}