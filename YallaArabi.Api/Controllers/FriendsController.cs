using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Friends.GetFriends;
using YallaArabi.Application.Features.Friends.SendFriendRequest;
using YallaArabi.Application.Features.Friends.RespondToFriendRequest;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/friends")]
[Authorize]
public class FriendsController : ControllerBase
{
    private readonly ISender _sender;

    public FriendsController(ISender sender) => _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetFriends(CancellationToken ct)
    {
        var result = await _sender.Send(new GetFriendsQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<FriendsListResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<FriendsListResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("requests")]
    public async Task<IActionResult> SendFriendRequest(SendFriendRequestCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SendFriendRequestResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SendFriendRequestResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("requests/{friendshipId}/respond")]
    public async Task<IActionResult> RespondToFriendRequest(
        Guid friendshipId,
        [FromBody] RespondToRequestBody body,
        CancellationToken ct)
    {
        var command = new RespondToFriendRequestCommand(friendshipId, body.Accept);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = body.Accept ? "Friend request accepted." : "Friend request declined." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}

public sealed record RespondToRequestBody(bool Accept);