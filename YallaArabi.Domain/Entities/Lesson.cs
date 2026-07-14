using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Lesson : Entity
{
    public Guid CategoryId { get; private set; }
    public string Title { get; private set; } = default!;
    public int OrderIndex { get; private set; }
    public int PointsReward { get; private set; }

    private Lesson() { }

    public static Lesson Create(Guid id, Guid categoryId, string title, int orderIndex, int pointsReward = 10)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Lesson title is required", nameof(title));

        if (pointsReward < 0)
            throw new ArgumentException("Points reward cannot be negative", nameof(pointsReward));

        return new Lesson
        {
            Id = id,
            CategoryId = categoryId,
            Title = title,
            OrderIndex = orderIndex,
            PointsReward = pointsReward
        };
    }
}