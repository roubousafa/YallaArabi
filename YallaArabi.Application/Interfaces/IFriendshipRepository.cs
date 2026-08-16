using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IFriendshipRepository
{
    Task<Friendship?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Friendship?> GetBetweenUsersAsync(Guid userAId, Guid userBId, CancellationToken ct = default);
    Task<IReadOnlyList<Friendship>> GetAcceptedForUserAsync(Guid userId, CancellationToken ct = default);
    Task<IReadOnlyList<Friendship>> GetPendingRequestsForUserAsync(Guid userId, CancellationToken ct = default);
    Task<bool> AreFriendsAsync(Guid userAId, Guid userBId, CancellationToken ct = default);
    Task AddAsync(Friendship friendship, CancellationToken ct = default);
    Task RemoveAsync(Friendship friendship, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}