using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class PointTransaction : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string ActionKey { get; private set; } = default!;
    public int Points { get; private set; }
    public Guid? RelatedEntityId { get; private set; }
    public string? Note { get; private set; }
    public DateTime EarnedAt { get; private set; }

    private PointTransaction() { }

    public static PointTransaction Create(Guid userId, string actionKey, int points, Guid? relatedEntityId = null, string? note = null)
    {
        if (string.IsNullOrWhiteSpace(actionKey))
            throw new ArgumentException("Action key is required", nameof(actionKey));

        return new PointTransaction
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ActionKey = actionKey,
            Points = points,
            RelatedEntityId = relatedEntityId,
            Note = note,
            EarnedAt = DateTime.UtcNow
        };
    }
}