using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Levels.CompleteLesson;
using YallaArabi.Application.Features.Levels.GetLessonContent;
using YallaArabi.Application.Features.Levels.MasterLessonWord;
using YallaArabi.Application.Features.Reviews.SubmitReview;
using YallaArabi.Application.Features.Levels.GetLevelDetails;
using YallaArabi.Application.Features.Levels.GetLevelsMap;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/levels")]
public class LevelsController : ControllerBase
{
    private readonly ISender _sender;

    public LevelsController(ISender sender) => _sender = sender;

    [Authorize]
    [HttpGet("map")]
    public async Task<IActionResult> GetLevelsMap(CancellationToken ct)
    {
        var result = await _sender.Send(new GetLevelsMapQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<LevelMapItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<LevelMapItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpGet("{levelCode}")]
    public async Task<IActionResult> GetLevelDetails(string levelCode, CancellationToken ct)
    {
        var result = await _sender.Send(new GetLevelDetailsQuery(levelCode), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<LevelDetailsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<LevelDetailsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpGet("lessons/{lessonId}")]
    public async Task<IActionResult> GetLessonContent(Guid lessonId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetLessonContentQuery(lessonId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<LessonContentResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<LessonContentResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("~/api/v1/lessons/words/{wordId}/master")]
    public async Task<IActionResult> MasterLessonWord(Guid wordId, [FromBody] SubmitReviewBody body, CancellationToken ct)
    {
        var result = await _sender.Send(new MasterLessonWordCommand(wordId, body.Grade), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<MasterLessonWordResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<MasterLessonWordResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("lessons/{lessonId}/complete")]
    public async Task<IActionResult> CompleteLesson(Guid lessonId, CancellationToken ct)
    {
        var result = await _sender.Send(new CompleteLessonCommand(lessonId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CompleteLessonResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CompleteLessonResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}