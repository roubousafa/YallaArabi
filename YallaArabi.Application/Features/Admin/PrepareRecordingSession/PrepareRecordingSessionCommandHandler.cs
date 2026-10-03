using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.PrepareRecordingSession;

public sealed class PrepareRecordingSessionCommandHandler
    : IRequestHandler<PrepareRecordingSessionCommand, Result<PrepareRecordingSessionResponse>>
{
    private const int MaxAllowedWords = 1000;

    private readonly IRecordingSessionRepository _sessionRepository;
    private readonly IWordAudioRepository _audioRepository;
    private readonly IWordRepository _wordRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ICurrentUserService _currentUserService;

    public PrepareRecordingSessionCommandHandler(
        IRecordingSessionRepository sessionRepository,
        IWordAudioRepository audioRepository,
        IWordRepository wordRepository,
        ILevelRepository levelRepository,
        ICurrentUserService currentUserService)
    {
        _sessionRepository = sessionRepository;
        _audioRepository = audioRepository;
        _wordRepository = wordRepository;
        _levelRepository = levelRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<PrepareRecordingSessionResponse>> Handle(
        PrepareRecordingSessionCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<PrepareRecordingSessionResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        if (!Dialects.IsSupported(request.Dialect))
        {
            return Result<PrepareRecordingSessionResponse>.Failure("invalid_dialect", "Unsupported dialect");
        }

        if (!VoiceGenders.IsSupported(request.Gender))
        {
            return Result<PrepareRecordingSessionResponse>.Failure("invalid_gender", "Unsupported voice gender");
        }

        var dialect = Dialects.Normalize(request.Dialect);
        var gender = VoiceGenders.Normalize(request.Gender);
        var limit = request.MaxWords < 1 ? 500 : Math.Min(request.MaxWords, MaxAllowedWords);

        Guid? levelId = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

            if (level is null)
            {
                return Result<PrepareRecordingSessionResponse>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
        }

        var (allWords, _) = await _wordRepository.GetPagedAsync(null, levelId, 1, MaxAllowedWords, cancellationToken);
        var alreadyRecorded = (await _audioRepository.GetWordIdsWithAudioAsync(dialect, gender, cancellationToken)).ToHashSet();

        var pending = allWords
            .Where(w => !alreadyRecorded.Contains(w.Id))
            .Take(limit)
            .ToList();

        if (pending.Count == 0)
        {
            return Result<PrepareRecordingSessionResponse>.Failure(
                "nothing_to_record",
                "All words already have audio for this dialect and gender");
        }

        var session = RecordingSession.Create(dialect, gender, pending.Count, currentUserId.Value);
        await _sessionRepository.AddAsync(session, cancellationToken);

        var items = new List<RecordingSessionItem>();
        var results = new List<SessionWordItem>();
        var order = 1;

        foreach (var word in pending)
        {
            items.Add(RecordingSessionItem.Create(session.Id, word.Id, order, word.ArabicText));

            results.Add(new SessionWordItem(order, word.Id, word.ArabicText, word.EnglishMeaning));
            order++;
        }

        await _sessionRepository.AddItemsAsync(items, cancellationToken);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        return Result<PrepareRecordingSessionResponse>.Success(
            new PrepareRecordingSessionResponse(
                session.Id, dialect, gender, pending.Count, session.Status, results));
    }
}