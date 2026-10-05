using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

// One row per student per notification: the in-app inbox, kept even when push delivery is skipped.
public sealed class UserNotification : Entity
{
    public Guid UserId { get; private set; }
    public string Type { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public Guid? RelatedEntityId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReadAt { get; private set; }

    private UserNotification() { }

    public static UserNotification Create(Guid userId, string type, string title, string body, Guid? relatedEntityId = null)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentException("Notification type is required", nameof(type));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Notification title is required", nameof(title));

        if (string.IsNullOrWhiteSpace(body))
            throw new ArgumentException("Notification body is required", nameof(body));

        return new UserNotification
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Type = type,
            Title = title.Trim(),
            Body = body.Trim(),
            RelatedEntityId = relatedEntityId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsRead => ReadAt.HasValue;

    public void MarkAsRead()
    {
        ReadAt ??= DateTime.UtcNow;
    }
}
