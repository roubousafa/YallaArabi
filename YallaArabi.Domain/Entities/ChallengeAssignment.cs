using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class ChallengeAssignment : AggregateRoot
{
    public Guid ChallengeId { get; private set; }
    public Guid UserId { get; private set; }
    public Guid AssignedByUserId { get; private set; }
    public DateTime AssignedAt { get; private set; }

    private ChallengeAssignment() { }

    public static ChallengeAssignment Create(Guid challengeId, Guid userId, Guid assignedByUserId)
    {
        return new ChallengeAssignment
        {
            Id = Guid.NewGuid(),
            ChallengeId = challengeId,
            UserId = userId,
            AssignedByUserId = assignedByUserId,
            AssignedAt = DateTime.UtcNow
        };
    }
}