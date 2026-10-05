using FluentValidation;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.AddLessonWords;

public sealed record AddLessonWordsCommand(Guid LessonId, IReadOnlyList<Guid> WordIds) : IRequest<Result<AddLessonWordsResponse>>;

public sealed record AddLessonWordsResponse(Guid LessonId, int Added, int AlreadyInLesson, int TotalWords);

public sealed class AddLessonWordsCommandValidator : AbstractValidator<AddLessonWordsCommand>
{
    public AddLessonWordsCommandValidator()
    {
        RuleFor(x => x.WordIds)
            .NotEmpty().WithMessage("At least one word is required");
    }
}

public sealed class AddLessonWordsCommandHandler : IRequestHandler<AddLessonWordsCommand, Result<AddLessonWordsResponse>>
{
    private const int MaxWordsPerLesson = 50;

    private readonly ILessonRepository _lessonRepository;
    private readonly ILessonContentRepository _contentRepository;
    private readonly IWordRepository _wordRepository;

    public AddLessonWordsCommandHandler(
        ILessonRepository lessonRepository,
        ILessonContentRepository contentRepository,
        IWordRepository wordRepository)
    {
        _lessonRepository = lessonRepository;
        _contentRepository = contentRepository;
        _wordRepository = wordRepository;
    }

    public async ValueTask<Result<AddLessonWordsResponse>> Handle(AddLessonWordsCommand request, CancellationToken cancellationToken)
    {
        var lesson = await _lessonRepository.GetByIdAsync(request.LessonId, cancellationToken);

        if (lesson is null)
        {
            return Result<AddLessonWordsResponse>.Failure("lesson_not_found", "Lesson not found");
        }

        var requestedIds = request.WordIds.Distinct().ToList();
        var foundIds = (await _wordRepository.GetByIdsAsync(requestedIds, cancellationToken)).Select(w => w.Id).ToHashSet();
        var missingIds = requestedIds.Where(id => !foundIds.Contains(id)).ToList();

        if (missingIds.Count > 0)
        {
            return Result<AddLessonWordsResponse>.Failure(
                "word_not_found",
                $"These words were not found: {string.Join(", ", missingIds)}");
        }

        var existing = await _contentRepository.GetWordsAsync(lesson.Id, cancellationToken);
        var existingIds = existing.Select(w => w.WordId).ToHashSet();
        var newIds = requestedIds.Where(id => !existingIds.Contains(id)).ToList();

        if (existing.Count + newIds.Count > MaxWordsPerLesson)
        {
            return Result<AddLessonWordsResponse>.Failure(
                "max_words_reached",
                $"A lesson cannot have more than {MaxWordsPerLesson} words");
        }

        var nextOrder = existing.Count == 0 ? 0 : existing.Max(w => w.OrderIndex) + 1;
        var links = newIds.Select((wordId, i) => LessonWord.Create(lesson.Id, wordId, nextOrder + i)).ToList();

        await _contentRepository.AddWordsAsync(links, cancellationToken);
        await _contentRepository.SaveChangesAsync(cancellationToken);

        return Result<AddLessonWordsResponse>.Success(new AddLessonWordsResponse(
            lesson.Id,
            links.Count,
            requestedIds.Count - newIds.Count,
            existing.Count + links.Count));
    }
}
