using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Challenges.GetAvailableChallenges;
using YallaArabi.Application.Features.Challenges.StartChallenge;
using YallaArabi.Application.Features.Challenges.SubmitChallenge;
using YallaArabi.Application.Features.Challenges.GetChallengeComparison;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/challenges")]
[Authorize]
public class ChallengesController : ControllerBase
{
    private readonly ISender _sender;

    public ChallengesController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetAvailableChallenges(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAvailableChallengesQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<AvailableChallengeItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<AvailableChallengeItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("{challengeId}/start")]
    public async Task<IActionResult> StartChallenge(Guid challengeId, CancellationToken ct)
    {
        var result = await _sender.Send(new StartChallengeCommand(challengeId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<StartChallengeResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<StartChallengeResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("participations/{participationId}/submit")]
    public async Task<IActionResult> SubmitChallenge(
        Guid participationId,
        [FromBody] SubmitChallengeBody body,
        CancellationToken ct)
    {
        var command = new SubmitChallengeCommand(participationId, body.Answers);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SubmitChallengeResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SubmitChallengeResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("{challengeId}/comparison/{opponentUserId}")]
    public async Task<IActionResult> GetComparison(Guid challengeId, Guid opponentUserId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetChallengeComparisonQuery(challengeId, opponentUserId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<ChallengeComparisonResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<ChallengeComparisonResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}

public sealed record SubmitChallengeBody(IReadOnlyList<SubmittedAnswer> Answers);