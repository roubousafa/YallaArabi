using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class LessonContentRepository : ILessonContentRepository
{
    private readonly AppDbContext _context;

    public LessonContentRepository(AppDbContext context) => _context = context;

    public async Task<IReadOnlyList<LessonWord>> GetWordsAsync(Guid lessonId, CancellationToken ct = default) =>
        await _context.LessonWords
            .Where(w => w.LessonId == lessonId)
            .OrderBy(w => w.OrderIndex)
            .ToListAsync(ct);

    public async Task<LessonWord?> GetWordAsync(Guid lessonId, Guid wordId, CancellationToken ct = default) =>
        await _context.LessonWords.FirstOrDefaultAsync(w => w.LessonId == lessonId && w.WordId == wordId, ct);

    public async Task AddWordsAsync(IEnumerable<LessonWord> words, CancellationToken ct = default) =>
        await _context.LessonWords.AddRangeAsync(words, ct);

    public void RemoveWord(LessonWord word) => _context.LessonWords.Remove(word);

    public async Task<IReadOnlyList<LessonQuestion>> GetQuestionsAsync(Guid lessonId, CancellationToken ct = default) =>
        await _context.LessonQuestions
            .Where(q => q.LessonId == lessonId)
            .OrderBy(q => q.OrderIndex)
            .ToListAsync(ct);

    public async Task<LessonQuestion?> GetQuestionByIdAsync(Guid questionId, CancellationToken ct = default) =>
        await _context.LessonQuestions.FirstOrDefaultAsync(q => q.Id == questionId, ct);

    public async Task<int> CountQuestionsAsync(Guid lessonId, CancellationToken ct = default) =>
        await _context.LessonQuestions.CountAsync(q => q.LessonId == lessonId, ct);

    public async Task AddQuestionAsync(LessonQuestion question, CancellationToken ct = default) =>
        await _context.LessonQuestions.AddAsync(question, ct);

    public void RemoveQuestion(LessonQuestion question) => _context.LessonQuestions.Remove(question);

    public async Task RemoveAllForLessonAsync(Guid lessonId, CancellationToken ct = default)
    {
        _context.LessonWords.RemoveRange(await _context.LessonWords.Where(w => w.LessonId == lessonId).ToListAsync(ct));
        _context.LessonQuestions.RemoveRange(await _context.LessonQuestions.Where(q => q.LessonId == lessonId).ToListAsync(ct));
    }

    public async Task<bool> IsWordUsedInQuestionsAsync(Guid wordId, CancellationToken ct = default) =>
        await _context.LessonQuestions.AnyAsync(q => q.WordId == wordId, ct);

    public async Task RemoveWordFromAllLessonsAsync(Guid wordId, CancellationToken ct = default) =>
        _context.LessonWords.RemoveRange(await _context.LessonWords.Where(w => w.WordId == wordId).ToListAsync(ct));

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}
