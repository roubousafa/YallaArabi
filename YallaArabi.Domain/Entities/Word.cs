using YallaArabi.Domain.Common;
using YallaArabi.Domain.Services;

namespace YallaArabi.Domain.Entities;

public sealed class Word : Entity
{
    public Guid LevelId { get; private set; }
    public string ArabicText { get; private set; } = default!;
    public string Transliteration { get; private set; } = default!;
    public string EnglishMeaning { get; private set; } = default!;
    public string NormalizedArabic { get; private set; } = default!;
    public string NormalizedTransliteration { get; private set; } = default!;
    public string NormalizedEnglish { get; private set; } = default!;
    public string? AudioUrl { get; private set; }
    public string? ImageUrl { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Word() { }

    public static Word Create(
        Guid levelId,
        string arabicText,
        string transliteration,
        string englishMeaning,
        string? audioUrl = null,
        string? imageUrl = null,
        Guid? id = null)
    {
        if (string.IsNullOrWhiteSpace(arabicText))
            throw new ArgumentException("Arabic text is required", nameof(arabicText));

        if (string.IsNullOrWhiteSpace(transliteration))
            throw new ArgumentException("Transliteration is required", nameof(transliteration));

        if (string.IsNullOrWhiteSpace(englishMeaning))
            throw new ArgumentException("English meaning is required", nameof(englishMeaning));

        return new Word
        {
            Id = id ?? Guid.NewGuid(),
            LevelId = levelId,
            ArabicText = arabicText.Trim(),
            Transliteration = transliteration.Trim(),
            EnglishMeaning = englishMeaning.Trim(),
            NormalizedArabic = TextNormalizer.NormalizeArabic(arabicText),
            NormalizedTransliteration = TextNormalizer.NormalizeLatin(transliteration),
            NormalizedEnglish = TextNormalizer.NormalizeLatin(englishMeaning),
            AudioUrl = audioUrl,
            ImageUrl = imageUrl,
            CreatedAt = DateTime.UtcNow
        };
    }
}