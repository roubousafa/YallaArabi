using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class WordAudio : AggregateRoot
{
    public Guid WordId { get; private set; }
    public Guid? VoiceProfileId { get; private set; }
    public string Dialect { get; private set; } = default!;
    public string Gender { get; private set; } = default!;
    public string Source { get; private set; } = default!;
    public string AudioUrl { get; private set; } = default!;
    public double? DurationSeconds { get; private set; }
    public string Status { get; private set; } = default!;
    public string? ReviewNote { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private WordAudio() { }

    public static WordAudio Create(
        Guid wordId,
        string dialect,
        string gender,
        string source,
        string audioUrl,
        Guid? voiceProfileId = null,
        double? durationSeconds = null,
        string status = "approved")
    {
        if (string.IsNullOrWhiteSpace(audioUrl))
            throw new ArgumentException("Audio URL is required", nameof(audioUrl));

        if (!Dialects.IsSupported(dialect))
            throw new ArgumentException("Unsupported dialect", nameof(dialect));

        if (!VoiceGenders.IsSupported(gender))
            throw new ArgumentException("Unsupported gender", nameof(gender));

        if (!AudioSources.IsSupported(source))
            throw new ArgumentException("Unsupported audio source", nameof(source));

        return new WordAudio
        {
            Id = Guid.NewGuid(),
            WordId = wordId,
            VoiceProfileId = voiceProfileId,
            Dialect = Dialects.Normalize(dialect),
            Gender = VoiceGenders.Normalize(gender),
            Source = source,
            AudioUrl = audioUrl.Trim(),
            DurationSeconds = durationSeconds,
            Status = status,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Approve()
    {
        Status = "approved";
        ReviewNote = null;
    }

    public void FlagForReview(string reason)
    {
        Status = "needs_review";
        ReviewNote = reason;
    }

    public void ReplaceAudio(string audioUrl, double? durationSeconds)
    {
        if (string.IsNullOrWhiteSpace(audioUrl))
            throw new ArgumentException("Audio URL is required", nameof(audioUrl));

        AudioUrl = audioUrl.Trim();
        DurationSeconds = durationSeconds;
    }

    public bool IsApproved() => Status == "approved";
}