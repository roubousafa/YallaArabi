using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Admin.AddWord;
using YallaArabi.Application.Features.Admin.BulkImportWords;
using YallaArabi.Application.Features.Admin.DeleteWord;
using YallaArabi.Application.Features.Admin.GetWordsList;
using YallaArabi.Application.Features.Admin.UpdateWord;
using YallaArabi.Application.Interfaces;
using YallaArabi.Application.Features.Admin.GetLevelsList;
using YallaArabi.Application.Features.Admin.AddLevel;
using YallaArabi.Application.Features.Admin.UpdateLevel;
using YallaArabi.Application.Features.Admin.DeleteLevel;
using YallaArabi.Application.Features.Admin.GetUsersList;
using YallaArabi.Application.Features.Admin.GetStudentDetails;
using YallaArabi.Application.Features.Admin.CreateUser;
using YallaArabi.Application.Features.Admin.UpdateUser;
using YallaArabi.Application.Features.Admin.DeleteUser;
using YallaArabi.Application.Features.Admin.GetSubscriptionsList;
using YallaArabi.Application.Features.Admin.GrantSubscription;
using YallaArabi.Application.Features.Admin.CancelSubscription;
using YallaArabi.Application.Features.Admin.SendCustomNotification;
using YallaArabi.Application.Features.Admin.GetNotificationsList;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly ISender _sender;

    public AdminController(ISender sender) => _sender = sender;

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(ApiResponse<object>.Ok(new { message = "You are an authenticated Admin! 🎉" }));
    }

    [HttpGet("words")]
    public async Task<IActionResult> GetWords(
        [FromQuery] string? searchTerm,
        [FromQuery] string? levelCode,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetWordsListQuery(searchTerm, levelCode, pageNumber, pageSize), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<PagedWordsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<PagedWordsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("words")]
    public async Task<IActionResult> AddWord(AddWordCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AddWordResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AddWordResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPut("words/{wordId}")]
    public async Task<IActionResult> UpdateWord(Guid wordId, [FromBody] UpdateWordBody body, CancellationToken ct)
    {
        var command = new UpdateWordCommand(
            wordId,
            body.LevelCode,
            body.ArabicText,
            body.Transliteration,
            body.EnglishMeaning,
            body.AudioUrl,
            body.ImageUrl);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<UpdateWordResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<UpdateWordResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpDelete("words/{wordId}")]
    public async Task<IActionResult> DeleteWord(Guid wordId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteWordCommand(wordId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Word deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("words/excel-template")]
    public IActionResult DownloadExcelTemplate([FromServices] IExcelTemplateGenerator generator)
    {
        var fileBytes = generator.GenerateWordsTemplate();

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "YallaArabi-Words-Template.xlsx");
    }

    [HttpPost("words/bulk-import")]
    [RequestSizeLimit(10_000_000)]
    public async Task<IActionResult> BulkImportWords(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.Fail("file_required", "Please upload a valid Excel file."));
        }

        await using var stream = file.OpenReadStream();
        var result = await _sender.Send(new BulkImportWordsCommand(stream), ct);

        return Ok(ApiResponse<BulkImportResult>.Ok(result));
    }

    [HttpGet("levels")]
    public async Task<IActionResult> GetLevels(CancellationToken ct)
    {
        var result = await _sender.Send(new GetLevelsListQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<AdminLevelItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<AdminLevelItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("levels")]
    public async Task<IActionResult> AddLevel(AddLevelCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AddLevelResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AddLevelResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPut("levels/{levelId}")]
    public async Task<IActionResult> UpdateLevel(Guid levelId, [FromBody] UpdateLevelBody body, CancellationToken ct)
    {
        var command = new UpdateLevelCommand(levelId, body.Name, body.OrderIndex);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<UpdateLevelResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<UpdateLevelResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpDelete("levels/{levelId}")]
    public async Task<IActionResult> DeleteLevel(Guid levelId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteLevelCommand(levelId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Level deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] string? searchTerm,
        [FromQuery] string? role,
        [FromQuery] string? levelCode,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetUsersListQuery(searchTerm, role, levelCode, pageNumber, pageSize), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<PagedUsersResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<PagedUsersResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetStudentDetails(Guid userId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetStudentDetailsQuery(userId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<StudentDetailsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<StudentDetailsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(CreateUserCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CreateUserResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CreateUserResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPut("users/{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserBody body, CancellationToken ct)
    {
        var command = new UpdateUserCommand(userId, body.FullName, body.Role, body.LevelCode);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<UpdateUserResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<UpdateUserResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpDelete("users/{userId}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteUserCommand(userId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "User deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("subscriptions")]
    public async Task<IActionResult> GetSubscriptions(
        [FromQuery] string? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetSubscriptionsListQuery(status, pageNumber, pageSize), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<PagedSubscriptionsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<PagedSubscriptionsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("subscriptions/grant")]
    public async Task<IActionResult> GrantSubscription(GrantSubscriptionCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<GrantSubscriptionResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<GrantSubscriptionResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("subscriptions/{subscriptionId}/cancel")]
    public async Task<IActionResult> CancelSubscription(Guid subscriptionId, CancellationToken ct)
    {
        var result = await _sender.Send(new CancelSubscriptionCommand(subscriptionId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Subscription cancelled successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("notifications")]
    public async Task<IActionResult> GetNotifications(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetNotificationsListQuery(pageNumber, pageSize), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<PagedNotificationsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<PagedNotificationsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("notifications/send")]
    public async Task<IActionResult> SendNotification(SendCustomNotificationCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SendCustomNotificationResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SendCustomNotificationResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}

public sealed record UpdateWordBody(
    string LevelCode,
    string ArabicText,
    string Transliteration,
    string EnglishMeaning,
    string? AudioUrl,
    string? ImageUrl
);

public sealed record UpdateLevelBody(string Name, int OrderIndex);

public sealed record UpdateUserBody(string FullName, string Role, string? LevelCode);