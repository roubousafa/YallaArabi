using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Lesson : Entity
{
    public Guid CategoryId { get; private set; }
    public string Title { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? IconUrl { get; private set; }
    public int OrderIndex { get; private set; }
    public int PointsReward { get; private set; }

    private Lesson() { }

    public static Lesson Create(
        Guid id,
        Guid categoryId,
        string title,
        int orderIndex,
        int pointsReward = 10,
        string? description = null,
        string? iconUrl = null)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Lesson title is required", nameof(title));

        if (pointsReward < 0)
            throw new ArgumentException("Points reward cannot be negative", nameof(pointsReward));

        return new Lesson
        {
            Id = id,
            CategoryId = categoryId,
            Title = title.Trim(),
            Description = description?.Trim(),
            IconUrl = iconUrl?.Trim(),
            OrderIndex = orderIndex,
            PointsReward = pointsReward
        };
    }

    public void Update(string title, string? description, string? iconUrl, int orderIndex, int pointsReward)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Lesson title is required", nameof(title));

        if (pointsReward < 0)
            throw new ArgumentException("Points reward cannot be negative", nameof(pointsReward));

        Title = title.Trim();
        Description = description?.Trim();
        IconUrl = iconUrl?.Trim();
        OrderIndex = orderIndex;
        PointsReward = pointsReward;
    }

    public void MoveToCategory(Guid categoryId)
    {
        CategoryId = categoryId;
    }
}