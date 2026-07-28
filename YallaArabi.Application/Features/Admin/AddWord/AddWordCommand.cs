using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.AddWord;

public sealed record AddWordCommand(
    string LevelCode,
    string ArabicText,
    string Transliteration,
    string EnglishMeaning,
    string? AudioUrl,
    string? ImageUrl
) : IRequest<Result<AddWordResponse>>;

public sealed record AddWordResponse(Guid WordId, string ArabicText, string LevelCode);