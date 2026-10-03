namespace YallaArabi.Domain.Common;

public static class ActivityTypes
{
    public const string LessonCompleted = "lesson_completed";
    public const string WordReviewed = "word_reviewed";
    public const string ChallengeCompleted = "challenge_completed";

    // Estimated study time per activity, used for daily-goal progress until the app reports real session time.
    public const int LessonEstimatedSeconds = 300;
    public const int ReviewEstimatedSeconds = 15;
}
