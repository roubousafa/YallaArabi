using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class SavedWord : AggregateRoot
{
    public Guid UserId { get; private set; }
    public Guid WordId { get; private set; }
    public DateTime SavedAt { get; private set; }

    private SavedWord() { }

    public static SavedWord Create(Guid userId, Guid wordId)
    {
        return new SavedWord
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            WordId = wordId,
            SavedAt = DateTime.UtcNow
        };
    }
}