using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class UserActivity : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string ActivityType { get; private set; } = default!;
    public Guid RelatedEntityId { get; private set; }
    public DateOnly LocalDate { get; private set; }
    public int DurationSeconds { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private UserActivity() { }

    public static UserActivity Create(Guid userId, string activityType, Guid relatedEntityId, DateOnly localDate, int durationSeconds)
    {
        if (string.IsNullOrWhiteSpace(activityType))
            throw new ArgumentException("Activity type is required", nameof(activityType));

        if (durationSeconds < 0)
            throw new ArgumentException("Duration cannot be negative", nameof(durationSeconds));

        return new UserActivity
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ActivityType = activityType,
            RelatedEntityId = relatedEntityId,
            LocalDate = localDate,
            DurationSeconds = durationSeconds,
            OccurredAt = DateTime.UtcNow
        };
    }
}
