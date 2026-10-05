using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using YallaArabi.Application.Interfaces;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Services;

public sealed class AccountDeletionService : IAccountDeletionService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AccountDeletionService> _logger;
    private readonly IFileStorage _fileStorage;

    public AccountDeletionService(AppDbContext context, ILogger<AccountDeletionService> logger, IFileStorage fileStorage)
    {
        _context = context;
        _fileStorage = fileStorage;
        _logger = logger;
    }

    public async Task DeleteUserDataAsync(Guid userId, CancellationToken ct = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);

        try
        {
            var refreshTokens = await _context.RefreshTokens.Where(t => t.UserId == userId).ToListAsync(ct);
            _context.RefreshTokens.RemoveRange(refreshTokens);

            var savedWords = await _context.SavedWords.Where(s => s.UserId == userId).ToListAsync(ct);
            _context.SavedWords.RemoveRange(savedWords);

            var reviews = await _context.WordReviews.Where(r => r.UserId == userId).ToListAsync(ct);
            _context.WordReviews.RemoveRange(reviews);

            var progress = await _context.StudentLessonProgress.Where(p => p.UserId == userId).ToListAsync(ct);
            _context.StudentLessonProgress.RemoveRange(progress);

            var points = await _context.PointTransactions.Where(p => p.UserId == userId).ToListAsync(ct);
            _context.PointTransactions.RemoveRange(points);

            var activities = await _context.UserActivities.Where(a => a.UserId == userId).ToListAsync(ct);
            _context.UserActivities.RemoveRange(activities);

            var notifications = await _context.UserNotifications.Where(n => n.UserId == userId).ToListAsync(ct);
            _context.UserNotifications.RemoveRange(notifications);

            var friendships = await _context.Friendships
                .Where(f => f.RequesterId == userId || f.AddresseeId == userId)
                .ToListAsync(ct);
            _context.Friendships.RemoveRange(friendships);

            var assignments = await _context.ChallengeAssignments.Where(a => a.UserId == userId).ToListAsync(ct);
            _context.ChallengeAssignments.RemoveRange(assignments);

            var participations = await _context.ChallengeParticipations.Where(p => p.UserId == userId).ToListAsync(ct);
            var participationIds = participations.Select(p => p.Id).ToList();

            var answers = await _context.ChallengeAnswers
                .Where(a => participationIds.Contains(a.ParticipationId))
                .ToListAsync(ct);
            _context.ChallengeAnswers.RemoveRange(answers);
            _context.ChallengeParticipations.RemoveRange(participations);

            var subscriptions = await _context.Subscriptions.Where(s => s.UserId == userId).ToListAsync(ct);
            _context.Subscriptions.RemoveRange(subscriptions);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

            if (user is not null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            if (!string.IsNullOrWhiteSpace(user?.AvatarUrl))
            {
                await _fileStorage.DeleteAsync(user.AvatarUrl, ct);
            }

            _logger.LogInformation("Account data deleted for user {UserId}", userId);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            _logger.LogError(ex, "Failed to delete account data for user {UserId}", userId);
            throw;
        }
    }
}