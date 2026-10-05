using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Notifications.GetMyNotifications;
using YallaArabi.Application.Features.Notifications.GetUnreadCount;
using YallaArabi.Application.Features.Notifications.MarkAllNotificationsRead;
using YallaArabi.Application.Features.Notifications.MarkNotificationRead;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/notifications")]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetMyNotifications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetMyNotificationsQuery(pageNumber, pageSize), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<MyNotificationsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<MyNotificationsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct)
    {
        var result = await _sender.Send(new GetUnreadCountQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<UnreadCountResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<UnreadCountResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, CancellationToken ct)
    {
        var result = await _sender.Send(new MarkNotificationReadCommand(notificationId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<UnreadCountResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<UnreadCountResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("read-all")]
    public async Task<IActionResult> MarkAllAsRead(CancellationToken ct)
    {
        var result = await _sender.Send(new MarkAllNotificationsReadCommand(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<MarkAllNotificationsReadResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<MarkAllNotificationsReadResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}
