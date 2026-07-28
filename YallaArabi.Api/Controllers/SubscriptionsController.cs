using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Subscriptions.CreateCheckout;
using YallaArabi.Application.Features.Subscriptions.GetSubscriptionStatus;
using YallaArabi.Application.Features.Subscriptions.ProcessWebhook;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISender _sender;

    public SubscriptionsController(ISender sender) => _sender = sender;

    [Authorize]
    [HttpGet("status")]
    public async Task<IActionResult> GetStatus(CancellationToken ct)
    {
        var result = await _sender.Send(new GetSubscriptionStatusQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SubscriptionStatusResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SubscriptionStatusResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("checkout")]
    public async Task<IActionResult> CreateCheckout(CreateCheckoutCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CreateCheckoutResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CreateCheckoutResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook([FromBody] ProcessWebhookCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Webhook processed successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}