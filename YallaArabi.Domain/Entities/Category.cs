using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Category : Entity
{
    public Guid LevelId { get; private set; }
    public string Name { get; private set; } = default!;
    public int OrderIndex { get; private set; }

    private Category() { }

    public static Category Create(Guid id, Guid levelId, string name, int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name is required", nameof(name));

        return new Category
        {
            Id = id,
            LevelId = levelId,
            Name = name,
            OrderIndex = orderIndex
        };
    }
}