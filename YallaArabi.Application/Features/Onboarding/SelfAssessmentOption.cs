namespace YallaArabi.Application.Features.Onboarding;

public sealed record SelfAssessmentOption(string Key, string Description, string LevelCode);

public static class SelfAssessmentOptions
{
    public static readonly IReadOnlyList<SelfAssessmentOption> All = new List<SelfAssessmentOption>
    {
        new("new_to_arabic", "I'm completely new to Arabic", "A0"),
        new("know_some_words", "I know some basic words and greetings", "A1"),
        new("simple_sentences", "I can form simple sentences", "A2"),
        new("basic_conversations", "I can have basic everyday conversations", "A3"),
        new("comfortable_conversations", "I can hold conversations comfortably on familiar topics", "A4"),
        new("advanced_topics", "I can discuss a wide range of topics with good fluency", "A5"),
        new("near_fluent", "I'm near fluent and want to refine my skills", "A6")
    };
}