namespace YallaArabi.Domain.Common;

public static class Dialects
{
    public const string Syrian = "syrian";
    public const string Lebanese = "lebanese";
    public const string Palestinian = "palestinian";
    public const string Jordanian = "jordanian";

    public const string DefaultDialect = Syrian;

    public static IReadOnlyList<DialectDefinition> All { get; } = new List<DialectDefinition>
    {
        new(Syrian, "سورية", "Syrian", "north"),
        new(Lebanese, "لبنانية", "Lebanese", "north"),
        new(Palestinian, "فلسطينية", "Palestinian", "south"),
        new(Jordanian, "أردنية", "Jordanian", "south"),
    };

    public static bool IsSupported(string? dialect) =>
        !string.IsNullOrWhiteSpace(dialect)
        && All.Any(d => d.Key == dialect.Trim().ToLowerInvariant());

    public static DialectDefinition? Get(string? dialect) =>
        string.IsNullOrWhiteSpace(dialect)
            ? null
            : All.FirstOrDefault(d => d.Key == dialect.Trim().ToLowerInvariant());

    public static string Normalize(string? dialect) =>
        IsSupported(dialect) ? dialect!.Trim().ToLowerInvariant() : DefaultDialect;
}

public sealed record DialectDefinition(
    string Key,
    string NameAr,
    string NameEn,
    string Region
);