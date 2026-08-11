using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Notification : AggregateRoot
{
    public string Title { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public string TargetType { get; private set; } = default!;
    public int RecipientsCount { get; private set; }
    public int DeliveredCount { get; private set; }
    public int SkippedCount { get; private set; }
    public Guid SentByUserId { get; private set; }
    public DateTime SentAt { get; private set; }

    private Notification() { }

    public static Notification Create(
        string title,
        string body,
        string targetType,
        int recipientsCount,
        int deliveredCount,
        int skippedCount,
        Guid sentByUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Notification title is required", nameof(title));

        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Notification body is required", nameof(body));

        return new Notification
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Body = body.Trim(),
            TargetType = targetType,
            RecipientsCount = recipientsCount,
            DeliveredCount = deliveredCount,
            SkippedCount = skippedCount,
            SentByUserId = sentByUserId,
            SentAt = DateTime.UtcNow
        };
    }
}