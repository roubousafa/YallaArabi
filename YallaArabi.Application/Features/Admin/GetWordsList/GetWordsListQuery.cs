using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.GetWordsList;

public sealed record GetWordsListQuery(
    string? SearchTerm,
    string? LevelCode,
    int PageNumber = 1,
    int PageSize = 20
) : IRequest<Result<PagedWordsResponse>>;

public sealed record PagedWordsResponse(
    IReadOnlyList<AdminWordItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

public sealed record AdminWordItem(
    Guid WordId,
    string ArabicText,
    string Transliteration,
    string EnglishMeaning,
    string LevelCode,
    string? AudioUrl,
    string? ImageUrl,
    DateTime CreatedAt
);