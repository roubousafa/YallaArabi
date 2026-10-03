using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UploadSessionAudio;

public sealed record UploadSessionAudioCommand(
    Guid SessionId,
    Stream AudioStream,
    string FileName,
    string ContentType
) : IRequest<Result<UploadSessionAudioResponse>>;

public sealed record UploadSessionAudioResponse(
    Guid SessionId,
    string AudioUrl,
    string Status,
    int WordsCount
);