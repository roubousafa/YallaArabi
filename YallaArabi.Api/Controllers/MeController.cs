using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Stats.GetDailyActivity;
using YallaArabi.Application.Features.Stats.GetMyStats;
using YallaArabi.Application.Features.Profile.GetMyProfile;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/me")]
[Authorize]
public class MeController : ControllerBase
{
    private readonly ISender _sender;

    public MeController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct)
    {
        var result = await _sender.Send(new GetMyProfileQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<MyProfileResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<MyProfileResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetMyStats(CancellationToken ct)
    {
        var result = await _sender.Send(new GetMyStatsQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<MyStatsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<MyStatsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("daily-activity")]
    public async Task<IActionResult> GetDailyActivity(CancellationToken ct)
    {
        var result = await _sender.Send(new GetDailyActivityQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<DailyActivityResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<DailyActivityResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}
