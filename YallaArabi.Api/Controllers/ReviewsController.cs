using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Notifications.SendReminderNotifications;
using YallaArabi.Application.Features.Reviews.GetDueReviews;
using YallaArabi.Application.Features.Reviews.GetStudentsNeedingReminder;
using YallaArabi.Application.Features.Reviews.StartReviewing;
using YallaArabi.Application.Features.Reviews.SubmitReview;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/reviews")]
public class ReviewsController : ControllerBase
{
    private readonly ISender _sender;

    public ReviewsController(ISender sender) => _sender = sender;

    [Authorize]
    [HttpPost("start")]
    public async Task<IActionResult> StartReviewing(CancellationToken ct)
    {
        var result = await _sender.Send(new StartReviewingCommand(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<StartReviewingResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<StartReviewingResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpGet("due")]
    public async Task<IActionResult> GetDueReviews(CancellationToken ct)
    {
        var result = await _sender.Send(new GetDueReviewsQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<DueReviewItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<DueReviewItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("{wordId}/submit")]
    public async Task<IActionResult> SubmitReview(Guid wordId, [FromBody] SubmitReviewBody body, CancellationToken ct)
    {
        var result = await _sender.Send(new SubmitReviewCommand(wordId, body.Grade), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SubmitReviewResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SubmitReviewResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("students-needing-reminder")]
    public async Task<IActionResult> GetStudentsNeedingReminder(
       [FromHeader(Name = "X-Internal-Api-Key")] string? apiKey,
       [FromServices] IConfiguration configuration,
       CancellationToken ct)
    {
        var expectedKey = configuration["InternalApi:SecretKey"];

        if (string.IsNullOrEmpty(expectedKey) || apiKey != expectedKey)
        {
            return Unauthorized(ApiResponse<object>.Fail("unauthorized", "Invalid or missing API key"));
        }

        var studentIds = await _sender.Send(new GetStudentsNeedingReminderQuery(), ct);

        return Ok(ApiResponse<IReadOnlyList<Guid>>.Ok(studentIds));
    }

    [HttpPost("send-reminders")]
    public async Task<IActionResult> SendReminders(
       [FromHeader(Name = "X-Internal-Api-Key")] string? apiKey,
       [FromServices] IConfiguration configuration,
       CancellationToken ct)
    {
        var expectedKey = configuration["InternalApi:SecretKey"];

        if (string.IsNullOrEmpty(expectedKey) || apiKey != expectedKey)
        {
            return Unauthorized(ApiResponse<object>.Fail("unauthorized", "Invalid or missing API key"));
        }

        var response = await _sender.Send(new SendReminderNotificationsCommand(), ct);

        return Ok(ApiResponse<SendReminderNotificationsResponse>.Ok(response));
    }
}

public sealed record SubmitReviewBody(ReviewGrade Grade);