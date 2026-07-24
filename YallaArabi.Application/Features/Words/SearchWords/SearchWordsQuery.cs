using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Words.SearchWords;

public sealed record SearchWordsQuery(string Query) : IRequest<Result<IReadOnlyList<WordSearchResult>>>;

public sealed record WordSearchResult(
    Guid WordId,
    string ArabicText,
    string Transliteration,
    string EnglishMeaning,
    string LevelCode,
    string? AudioUrl,
    string? ImageUrl
);