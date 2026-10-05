using YallaArabi.Domain.Entities;

namespace YallaArabi.Domain.Common;

public static class WordMastery
{
    public const string New = "new";
    public const string Learning = "learning";
    public const string Mastered = "mastered";

    // A word counts as mastered once SRS spaces its next review three weeks or more apart.
    public const int MasteredIntervalDays = 21;

    public static string Of(WordReview? review) =>
        review is null || review.LastReviewedAt is null
            ? New
            : review.IntervalDays >= MasteredIntervalDays ? Mastered : Learning;
}
