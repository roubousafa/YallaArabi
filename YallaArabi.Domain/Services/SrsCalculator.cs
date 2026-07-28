namespace YallaArabi.Domain.Services;

using YallaArabi.Domain.Entities;

public static class SrsCalculator
{
    private const double MinEaseFactor = 1.3;

    public static (int IntervalDays, double EaseFactor, int RepetitionCount) CalculateNext(
        ReviewGrade grade, int currentRepetitionCount, double currentEaseFactor, int currentIntervalDays)
    {
        if (grade == ReviewGrade.Hard)
        {
            return (IntervalDays: 1, EaseFactor: AdjustEase(currentEaseFactor, grade), RepetitionCount: 0);
        }

        var newRepetitionCount = currentRepetitionCount + 1;
        var newEaseFactor = AdjustEase(currentEaseFactor, grade);

        int newInterval = newRepetitionCount switch
        {
            1 => 1,
            2 => 3,
            _ => (int)Math.Round(currentIntervalDays * newEaseFactor)
        };

        return (newInterval, newEaseFactor, newRepetitionCount);
    }

    private static double AdjustEase(double currentEase, ReviewGrade grade)
    {
        var delta = grade switch
        {
            ReviewGrade.Hard => -0.3,
            ReviewGrade.Medium => 0.0,
            ReviewGrade.Easy => 0.15,
            _ => 0.0
        };

        var newEase = currentEase + delta;
        return Math.Max(newEase, MinEaseFactor);
    }
}