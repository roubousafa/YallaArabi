using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class WordAudioRepository : IWordAudioRepository
{
    private const string ApprovedStatus = "approved";

    private readonly AppDbContext _context;

    public WordAudioRepository(AppDbContext context) => _context = context;

    public async Task<WordAudio?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.WordAudios.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<WordAudio?> FindForWordAsync(Guid wordId, string dialect, string gender, CancellationToken ct = default)
    {
        var exact = await _context.WordAudios
            .FirstOrDefaultAsync(a =>
                a.WordId == wordId &&
                a.Dialect == dialect &&
                a.Gender == gender &&
                a.Status == ApprovedStatus, ct);

        if (exact is not null)
        {
            return exact;
        }

        return await _context.WordAudios
            .Where(a => a.WordId == wordId && a.Dialect == dialect && a.Status == ApprovedStatus)
            .OrderByDescending(a => a.Source == "human")
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<WordAudio>> GetByWordIdAsync(Guid wordId, CancellationToken ct = default) =>
        await _context.WordAudios
            .Where(a => a.WordId == wordId)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Guid>> GetWordIdsWithAudioAsync(string dialect, string gender, CancellationToken ct = default) =>
        await _context.WordAudios
            .Where(a => a.Dialect == dialect && a.Gender == gender)
            .Select(a => a.WordId)
            .Distinct()
            .ToListAsync(ct);

    public async Task AddRangeAsync(IEnumerable<WordAudio> audios, CancellationToken ct = default) =>
        await _context.WordAudios.AddRangeAsync(audios, ct);

    public async Task AddAsync(WordAudio audio, CancellationToken ct = default) =>
        await _context.WordAudios.AddAsync(audio, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}