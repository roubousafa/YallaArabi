namespace YallaArabi.Domain.Common;

public static class LessonQuestionTypes
{
    // Text question with 2-4 options.
    public const string MultipleChoice = "multiple_choice";

    // Student hears a word (question audio or the linked word's audio) and picks its meaning.
    public const string ListenChoose = "listen_choose";

    // Student hears the linked word and repeats it; no options.
    public const string SpeakRepeat = "speak_repeat";

    public static IReadOnlyList<string> All { get; } = new[] { MultipleChoice, ListenChoose, SpeakRepeat };

    public static bool IsSupported(string? type) =>
        !string.IsNullOrWhiteSpace(type) && All.Contains(type.Trim().ToLowerInvariant());

    public static bool HasOptions(string type) => type is MultipleChoice or ListenChoose;
}
