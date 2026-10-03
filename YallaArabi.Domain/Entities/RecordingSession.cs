using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class RecordingSession : AggregateRoot
{
    public string Dialect { get; private set; } = default!;
    public string Gender { get; private set; } = default!;
    public Guid? VoiceProfileId { get; private set; }
    public int WordsCount { get; private set; }
    public string Status { get; private set; } = default!;
    public string? UploadedAudioUrl { get; private set; }
    public int ClipsDetected { get; private set; }
    public int ClipsFlagged { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private RecordingSession() { }

    public static RecordingSession Create(
        string dialect,
        string gender,
        int wordsCount,
        Guid createdByUserId,
        Guid? voiceProfileId = null)
    {
        if (!Dialects.IsSupported(dialect))
            throw new ArgumentException("Unsupported dialect", nameof(dialect));

        if (!VoiceGenders.IsSupported(gender))
            throw new ArgumentException("Unsupported gender", nameof(gender));

        return new RecordingSession
        {
            Id = Guid.NewGuid(),
            Dialect = Dialects.Normalize(dialect),
            Gender = VoiceGenders.Normalize(gender),
            VoiceProfileId = voiceProfileId,
            WordsCount = wordsCount,
            Status = "prepared",
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MarkUploaded(string audioUrl)
    {
        UploadedAudioUrl = audioUrl;
        Status = "uploaded";
    }

    public void MarkProcessed(int clipsDetected, int clipsFlagged)
    {
        ClipsDetected = clipsDetected;
        ClipsFlagged = clipsFlagged;
        Status = "processed";
    }

    public void MarkCompleted()
    {
        Status = "completed";
        CompletedAt = DateTime.UtcNow;
    }

    public void AssignVoiceProfile(Guid voiceProfileId)
    {
        VoiceProfileId = voiceProfileId;
    }
}