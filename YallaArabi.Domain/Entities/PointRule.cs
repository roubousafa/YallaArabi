using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class PointRule : Entity
{
    public string ActionKey { get; private set; } = default!;
    public string DisplayName { get; private set; } = default!;
    public string? Description { get; private set; }
    public int Points { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private PointRule() { }

    public static PointRule Create(Guid id, string actionKey, string displayName, string? description, int points)
    {
        if (string.IsNullOrWhiteSpace(actionKey))
            throw new ArgumentException("Action key is required", nameof(actionKey));

        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name is required", nameof(displayName));

        if (points < 0)
            throw new ArgumentException("Points cannot be negative", nameof(points));

        return new PointRule
        {
            Id = id,
            ActionKey = actionKey,
            DisplayName = displayName,
            Description = description,
            Points = points,
            IsActive = true,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string displayName, string? description, int points, bool isActive)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name is required", nameof(displayName));

        if (points < 0)
            throw new ArgumentException("Points cannot be negative", nameof(points));

        DisplayName = displayName.Trim();
        Description = description?.Trim();
        Points = points;
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
}