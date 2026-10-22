using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Interfaces;

// Turns a student's search (Arabic, English or chat-alphabet) into a dictionary entry in their dialect.
public interface IDialectAiProvider
{
    string Name { get; }

    Task<DialectAiResult> GenerateAsync(string query, DialectDefinition dialect, CancellationToken ct = default);
}

// IsValid is false when the query isn't a word, phrase or letter the AI can explain, or isn't appropriate to teach.
// ImageQuery is a short English photo search, or null when the meaning can't be pictured (e.g. "but", a single letter).
public sealed record DialectAiResult(bool IsValid, string? DialectText, string? EnglishMeaning, string? Arabizi, string? ImageQuery = null);
