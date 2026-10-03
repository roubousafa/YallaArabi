namespace YallaArabi.Domain.Common;

public static class VoiceGenders
{
    public const string Male = "male";
    public const string Female = "female";

    public const string DefaultGender = Female;

    public static IReadOnlyList<VoiceGenderDefinition> All { get; } = new List<VoiceGenderDefinition>
    {
        new(Female, "أنثى", "Female"),
        new(Male, "ذكر", "Male"),
    };

    public static bool IsSupported(string? gender) =>
        !string.IsNullOrWhiteSpace(gender)
        && All.Any(g => g.Key == gender.Trim().ToLowerInvariant());

    public static string Normalize(string? gender) =>
        IsSupported(gender) ? gender!.Trim().ToLowerInvariant() : DefaultGender;
}

public sealed record VoiceGenderDefinition(
    string Key,
    string NameAr,
    string NameEn
);