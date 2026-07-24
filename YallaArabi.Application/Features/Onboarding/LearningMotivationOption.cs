namespace YallaArabi.Application.Features.Onboarding;

public sealed record LearningMotivationOption(string Key, string Title, string Description);

public static class LearningMotivationOptions
{
    public static readonly IReadOnlyList<LearningMotivationOption> All = new List<LearningMotivationOption>
    {
        new("family_heritage", "Family & heritage", "Connect with relatives and roots"),
        new("travel", "Travel", "Get around the Levant with ease"),
        new("living_region", "Living in the region", "I'm based here or moving soon"),
        new("work", "Work", "Arabic comes up in my job"),
        new("just_curious", "Just curious", "No particular reason")
    };
}