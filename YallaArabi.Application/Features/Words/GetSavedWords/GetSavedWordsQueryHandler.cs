using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Words.SearchWords;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Words.GetSavedWords;

public sealed class GetSavedWordsQueryHandler : IRequestHandler<GetSavedWordsQuery, Result<IReadOnlyList<WordSearchResult>>>
{
    private readonly ISavedWordRepository _savedWordRepository;
    private readonly IWordRepository _wordRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetSavedWordsQueryHandler(
        ISavedWordRepository savedWordRepository,
        IWordRepository wordRepository,
        ILevelRepository levelRepository,
        ICurrentUserService currentUserService)
    {
        _savedWordRepository = savedWordRepository;
        _wordRepository = wordRepository;
        _levelRepository = levelRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<IReadOnlyList<WordSearchResult>>> Handle(GetSavedWordsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<IReadOnlyList<WordSearchResult>>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var savedWords = await _savedWordRepository.GetByUserIdAsync(currentUserId.Value, cancellationToken);
        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        var results = new List<WordSearchResult>();

        foreach (var saved in savedWords)
        {
            var word = await _wordRepository.GetByIdAsync(saved.WordId, cancellationToken);

            if (word is not null)
            {
                results.Add(new WordSearchResult(
                    word.Id,
                    word.ArabicText,
                    word.Transliteration,
                    word.EnglishMeaning,
                    levelCodeById.TryGetValue(word.LevelId, out var code) ? code : "?",
                    word.AudioUrl,
                    word.ImageUrl));
            }
        }

        return Result<IReadOnlyList<WordSearchResult>>.Success(results);
    }
}