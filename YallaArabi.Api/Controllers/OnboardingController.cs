using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Onboarding;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/onboarding")]
public class OnboardingController : ControllerBase
{
    private readonly ISender _sender;

    public OnboardingController(ISender sender) => _sender = sender;

    [HttpGet("self-assessment-options")]
    public async Task<IActionResult> GetSelfAssessmentOptions(CancellationToken ct)
    {
        var options = await _sender.Send(new GetSelfAssessmentOptionsQuery(), ct);
        return Ok(ApiResponse<IReadOnlyList<SelfAssessmentOption>>.Ok(options));
    }

    [Authorize]
    [HttpPost("select-level")]
    public async Task<IActionResult> SelectLevel(SelectLevelCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SelectLevelResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SelectLevelResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("daily-goal-options")]
    public async Task<IActionResult> GetDailyGoalOptions(CancellationToken ct)
    {
        var options = await _sender.Send(new GetDailyGoalOptionsQuery(), ct);
        return Ok(ApiResponse<IReadOnlyList<DailyGoalOption>>.Ok(options));
    }

    [Authorize]
    [HttpPost("daily-goal")]
    public async Task<IActionResult> SetDailyGoal(SetDailyGoalCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SetDailyGoalResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SetDailyGoalResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("learning-motivation-options")]
    public async Task<IActionResult> GetLearningMotivationOptions(CancellationToken ct)
    {
        var options = await _sender.Send(new GetLearningMotivationOptionsQuery(), ct);
        return Ok(ApiResponse<IReadOnlyList<LearningMotivationOption>>.Ok(options));
    }

    [Authorize]
    [HttpPost("learning-motivation")]
    public async Task<IActionResult> SetLearningMotivation(SetLearningMotivationCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SetLearningMotivationResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SetLearningMotivationResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}