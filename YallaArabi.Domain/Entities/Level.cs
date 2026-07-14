using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Level : Entity
{
    public string Code { get; private set; } = default!;
    public string Name { get; private set; } = default!;
    public int OrderIndex { get; private set; }

    private Level() { }

    public static Level Create(Guid id, string code, string name, int orderIndex)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Level code is required", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Level name is required", nameof(name));

        return new Level
        {
            Id = id,
            Code = code,
            Name = name,
            OrderIndex = orderIndex
        };
    }
}