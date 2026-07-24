using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Services;

namespace YallaArabi.Application.Features.Words.SearchWords;

public sealed class SearchWordsQueryHandler : IRequestHandler<SearchWordsQuery, Result<IReadOnlyList<WordSearchResult>>>
{
    private const int MaxResults = 20;

    private readonly IWordRepository _wordRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public SearchWordsQueryHandler(
        IWordRepository wordRepository,
        ILevelRepository levelRepository,
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _wordRepository = wordRepository;
        _levelRepository = levelRepository;
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<IReadOnlyList<WordSearchResult>>> Handle(SearchWordsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<IReadOnlyList<WordSearchResult>>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var normalizedQuery = TextNormalizer.Normalize(request.Query);

        if (string.IsNullOrEmpty(normalizedQuery))
        {
            return Result<IReadOnlyList<WordSearchResult>>.Success(Array.Empty<WordSearchResult>());
        }

        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);
        IReadOnlyList<Guid>? allowedLevelIds = null;

        if (user?.CurrentLevelId is not null)
        {
            var currentLevel = allLevels.FirstOrDefault(l => l.Id == user.CurrentLevelId);
            if (currentLevel is not null)
            {
                allowedLevelIds = allLevels
                    .Where(l => l.OrderIndex <= currentLevel.OrderIndex)
                    .Select(l => l.Id)
                    .ToList();
            }
        }

        var words = await _wordRepository.SearchAsync(normalizedQuery, allowedLevelIds, MaxResults, cancellationToken);

        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        var results = words
            .Select(w => new WordSearchResult(
                w.Id,
                w.ArabicText,
                w.Transliteration,
                w.EnglishMeaning,
                levelCodeById.TryGetValue(w.LevelId, out var code) ? code : "?",
                w.AudioUrl,
                w.ImageUrl))
            .ToList();

        return Result<IReadOnlyList<WordSearchResult>>.Success(results);
    }
}