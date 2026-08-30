using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Category : Entity
{
    public Guid LevelId { get; private set; }
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? IconUrl { get; private set; }
    public int OrderIndex { get; private set; }

    private Category() { }

    public static Category Create(Guid id, Guid levelId, string name, int orderIndex, string? description = null, string? iconUrl = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required", nameof(name));

        return new Category
        {
            Id = id,
            LevelId = levelId,
            Name = name.Trim(),
            Description = description?.Trim(),
            IconUrl = iconUrl?.Trim(),
            OrderIndex = orderIndex
        };
    }

    public void Update(string name, string? description, string? iconUrl, int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required", nameof(name));

        Name = name.Trim();
        Description = description?.Trim();
        IconUrl = iconUrl?.Trim();
        OrderIndex = orderIndex;
    }

    public void MoveToLevel(Guid levelId)
    {
        LevelId = levelId;
    }
}