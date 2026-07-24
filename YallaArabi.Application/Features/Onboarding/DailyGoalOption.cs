namespace YallaArabi.Application.Features.Onboarding;

public sealed record DailyGoalOption(string Key, string Title, string Description, int Minutes);

public static class DailyGoalOptions
{
    public static readonly IReadOnlyList<DailyGoalOption> All = new List<DailyGoalOption>
    {
        new("casual", "Casual", "A light daily habit", 5),
        new("regular", "Regular", "Steady, consistent progress", 10),
        new("serious", "Serious", "Push a little harder each day", 15),
        new("intense", "Intense", "Fastest path to fluency", 20)
    };
}