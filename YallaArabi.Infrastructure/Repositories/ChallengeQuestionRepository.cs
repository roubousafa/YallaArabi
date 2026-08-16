using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class ChallengeQuestionRepository : IChallengeQuestionRepository
{
    private readonly AppDbContext _context;

    public ChallengeQuestionRepository(AppDbContext context) => _context = context;

    public async Task<ChallengeQuestion?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.ChallengeQuestions.FirstOrDefaultAsync(q => q.Id == id, ct);

    public async Task<IReadOnlyList<ChallengeQuestion>> GetByChallengeIdAsync(Guid challengeId, CancellationToken ct = default) =>
        await _context.ChallengeQuestions
            .Where(q => q.ChallengeId == challengeId)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync(ct);

    public async Task<int> CountByChallengeIdAsync(Guid challengeId, CancellationToken ct = default) =>
        await _context.ChallengeQuestions.CountAsync(q => q.ChallengeId == challengeId, ct);

    public async Task AddAsync(ChallengeQuestion question, CancellationToken ct = default) =>
        await _context.ChallengeQuestions.AddAsync(question, ct);

    public async Task RemoveAsync(ChallengeQuestion question, CancellationToken ct = default)
    {
        _context.ChallengeQuestions.Remove(question);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}