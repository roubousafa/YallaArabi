using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UploadSessionAudio;

public sealed class UploadSessionAudioCommandHandler
    : IRequestHandler<UploadSessionAudioCommand, Result<UploadSessionAudioResponse>>
{
    private readonly IRecordingSessionRepository _sessionRepository;
    private readonly IFileStorage _fileStorage;

    public UploadSessionAudioCommandHandler(
        IRecordingSessionRepository sessionRepository,
        IFileStorage fileStorage)
    {
        _sessionRepository = sessionRepository;
        _fileStorage = fileStorage;
    }

    public async ValueTask<Result<UploadSessionAudioResponse>> Handle(
        UploadSessionAudioCommand request, CancellationToken cancellationToken)
    {
        var session = await _sessionRepository.GetByIdAsync(request.SessionId, cancellationToken);

        if (session is null)
        {
            return Result<UploadSessionAudioResponse>.Failure("session_not_found", "Recording session not found");
        }

        if (session.Status == "completed")
        {
            return Result<UploadSessionAudioResponse>.Failure(
                "session_completed",
                "This session is already completed. Prepare a new session instead.");
        }

        var result = await _fileStorage.UploadAsync(
            request.AudioStream,
            request.FileName,
            request.ContentType,
            "sessions",
            cancellationToken);

        session.MarkUploaded(result.Url);
        await _sessionRepository.SaveChangesAsync(cancellationToken);

        return Result<UploadSessionAudioResponse>.Success(
            new UploadSessionAudioResponse(session.Id, result.Url, session.Status, session.WordsCount));
    }
}