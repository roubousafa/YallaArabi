using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Levels.CompleteLesson;
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
    [HttpPost("lessons/{lessonId}/complete")]
    public async Task<IActionResult> CompleteLesson(Guid lessonId, CancellationToken ct)
    {
        var result = await _sender.Send(new CompleteLessonCommand(lessonId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CompleteLessonResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CompleteLessonResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}