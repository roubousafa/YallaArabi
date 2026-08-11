using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateWord;

public sealed record UpdateWordCommand(
    Guid WordId,
    string LevelCode,
    string ArabicText,
    string Transliteration,
    string EnglishMeaning,
    string? AudioUrl,
    string? ImageUrl
) : IRequest<Result<UpdateWordResponse>>;

public sealed record UpdateWordResponse(Guid WordId, string ArabicText, string LevelCode);