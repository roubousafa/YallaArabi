using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Words.SearchWords;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Words.GetWordById;

public sealed class GetWordByIdQueryHandler : IRequestHandler<GetWordByIdQuery, Result<WordSearchResult>>
{
    private readonly IWordRepository _wordRepository;
    private readonly ILevelRepository _levelRepository;

    public GetWordByIdQueryHandler(IWordRepository wordRepository, ILevelRepository levelRepository)
    {
        _wordRepository = wordRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<WordSearchResult>> Handle(GetWordByIdQuery request, CancellationToken cancellationToken)
    {
        var word = await _wordRepository.GetByIdAsync(request.WordId, cancellationToken);

        if (word is null)
        {
            return Result<WordSearchResult>.Failure("word_not_found", "Word not found");
        }

        var level = await _levelRepository.GetByIdAsync(word.LevelId, cancellationToken);

        var result = new WordSearchResult(
            word.Id,
            word.ArabicText,
            word.Transliteration,
            word.EnglishMeaning,
            level?.Code ?? "?",
            word.AudioUrl,
            word.ImageUrl);

        return Result<WordSearchResult>.Success(result);
    }
}