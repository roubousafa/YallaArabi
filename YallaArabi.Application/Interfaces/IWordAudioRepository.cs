using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IWordAudioRepository
{
    Task<WordAudio?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<WordAudio?> FindForWordAsync(Guid wordId, string dialect, string gender, CancellationToken ct = default);
    Task<IReadOnlyList<WordAudio>> GetByWordIdAsync(Guid wordId, CancellationToken ct = default);
    Task<IReadOnlyList<Guid>> GetWordIdsWithAudioAsync(string dialect, string gender, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<WordAudio> audios, CancellationToken ct = default);
    Task AddAsync(WordAudio audio, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}