using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class RecordingSessionItem : Entity
{
    public Guid SessionId { get; private set; }
    public Guid WordId { get; private set; }
    public int OrderNumber { get; private set; }
    public string TextToRead { get; private set; } = default!;
    public string? ClipUrl { get; private set; }
    public double? DurationSeconds { get; private set; }
    public string Status { get; private set; } = default!;
    public string? FlagReason { get; private set; }

    private RecordingSessionItem() { }

    public static RecordingSessionItem Create(Guid sessionId, Guid wordId, int orderNumber, string textToRead)
    {
        return new RecordingSessionItem
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            WordId = wordId,
            OrderNumber = orderNumber,
            TextToRead = textToRead.Trim(),
            Status = "pending"
        };
    }

    public void AttachClip(string clipUrl, double durationSeconds)
    {
        ClipUrl = clipUrl;
        DurationSeconds = durationSeconds;
        Status = "matched";
        FlagReason = null;
    }

    public void Flag(string reason)
    {
        Status = "flagged";
        FlagReason = reason;
    }

    public void Approve()
    {
        Status = "approved";
        FlagReason = null;
    }

    public void Skip()
    {
        Status = "skipped";
    }
}