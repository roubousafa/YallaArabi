using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class ChallengeAnswerRepository : IChallengeAnswerRepository
{
    private readonly AppDbContext _context;

    public ChallengeAnswerRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<ChallengeAnswer>> GetByParticipationIdAsync(Guid participationId, CancellationToken ct = default) =>
        await _context.ChallengeAnswers
            .Where(a => a.ParticipationId == participationId)
            .OrderBy(a => a.OrderIndex)
            .ToListAsync(ct);

    public async Task AddRangeAsync(IEnumerable<ChallengeAnswer> answers, CancellationToken ct = default) =>
        await _context.ChallengeAnswers.AddRangeAsync(answers, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}