using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class FriendshipRepository : IFriendshipRepository
{
    private readonly AppDbContext _context;

    public FriendshipRepository(AppDbContext context) => _context = context;

    public async Task<Friendship?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Friendships.FirstOrDefaultAsync(f => f.Id == id, ct);

    public async Task<Friendship?> GetBetweenUsersAsync(Guid userAId, Guid userBId, CancellationToken ct = default) =>
        await _context.Friendships.FirstOrDefaultAsync(f =>
            (f.RequesterId == userAId && f.AddresseeId == userBId) ||
            (f.RequesterId == userBId && f.AddresseeId == userAId), ct);

    public async Task<IReadOnlyList<Friendship>> GetAcceptedForUserAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Friendships
            .Where(f => f.Status == "accepted" && (f.RequesterId == userId || f.AddresseeId == userId))
            .OrderByDescending(f => f.RespondedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Friendship>> GetPendingRequestsForUserAsync(Guid userId, CancellationToken ct = default) =>
        await _context.Friendships
            .Where(f => f.Status == "pending" && f.AddresseeId == userId)
            .OrderByDescending(f => f.RequestedAt)
            .ToListAsync(ct);

    public async Task<bool> AreFriendsAsync(Guid userAId, Guid userBId, CancellationToken ct = default) =>
        await _context.Friendships.AnyAsync(f =>
            f.Status == "accepted" &&
            ((f.RequesterId == userAId && f.AddresseeId == userBId) ||
             (f.RequesterId == userBId && f.AddresseeId == userAId)), ct);

    public async Task AddAsync(Friendship friendship, CancellationToken ct = default) =>
        await _context.Friendships.AddAsync(friendship, ct);

    public async Task RemoveAsync(Friendship friendship, CancellationToken ct = default)
    {
        _context.Friendships.Remove(friendship);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}