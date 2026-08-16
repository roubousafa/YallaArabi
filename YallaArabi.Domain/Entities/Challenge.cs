using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Challenge : AggregateRoot
{
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public Guid? LevelId { get; private set; }
    public int PointsReward { get; private set; }
    public int TimeLimitSeconds { get; private set; }
    public string Status { get; private set; } = default!;
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }

    private Challenge() { }

    public static Challenge Create(
        string title,
        string? description,
        Guid? levelId,
        int pointsReward,
        int timeLimitSeconds,
        Guid createdByUserId,
        DateTime? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Challenge title is required", nameof(title));

        if (pointsReward < 0)
            throw new ArgumentException("Points reward cannot be negative", nameof(pointsReward));

        if (timeLimitSeconds <= 0)
            throw new ArgumentException("Time limit must be greater than zero", nameof(timeLimitSeconds));

        return new Challenge
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            Description = description?.Trim(),
            LevelId = levelId,
            PointsReward = pointsReward,
            TimeLimitSeconds = timeLimitSeconds,
            Status = "draft",
            CreatedByUserId = createdByUserId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt
        };
    }

    public void Update(string title, string? description, Guid? levelId, int pointsReward, int timeLimitSeconds, DateTime? expiresAt)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Challenge title is required", nameof(title));

        if (pointsReward < 0)
            throw new ArgumentException("Points reward cannot be negative", nameof(pointsReward));

        if (timeLimitSeconds <= 0)
            throw new ArgumentException("Time limit must be greater than zero", nameof(timeLimitSeconds));

        Title = title.Trim();
        Description = description?.Trim();
        LevelId = levelId;
        PointsReward = pointsReward;
        TimeLimitSeconds = timeLimitSeconds;
        ExpiresAt = expiresAt;
    }

    public void Publish()
    {
        Status = "published";
    }

    public void Archive()
    {
        Status = "archived";
    }

    public bool IsAvailable() =>
        Status == "published" && (ExpiresAt is null || DateTime.UtcNow < ExpiresAt.Value);
}