using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.PrepareRecordingSession;

public sealed record PrepareRecordingSessionCommand(
    string Dialect,
    string Gender,
    int MaxWords = 500,
    string? LevelCode = null
) : IRequest<Result<PrepareRecordingSessionResponse>>;

public sealed record PrepareRecordingSessionResponse(
    Guid SessionId,
    string Dialect,
    string Gender,
    int WordsCount,
    string Status,
    IReadOnlyList<SessionWordItem> Words
);

public sealed record SessionWordItem(
    int OrderNumber,
    Guid WordId,
    string TextToRead,
    string EnglishMeaning
);