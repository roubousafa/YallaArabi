using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.ExportRecordingScript;

public sealed record ExportRecordingScriptQuery(Guid SessionId) : IRequest<Result<RecordingScriptResponse>>;

public sealed record RecordingScriptResponse(
    byte[] Content,
    string FileName,
    int WordsCount
);