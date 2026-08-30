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
using YallaArabi.Application.Features.Admin.GetPointRules;
using YallaArabi.Application.Features.Admin.AddPointRule;
using YallaArabi.Application.Features.Admin.UpdatePointRule;
using YallaArabi.Application.Features.Admin.CreateChallenge;
using YallaArabi.Application.Features.Admin.UpdateChallenge;
using YallaArabi.Application.Features.Admin.DeleteChallenge;
using YallaArabi.Application.Features.Admin.PublishChallenge;
using YallaArabi.Application.Features.Admin.GetChallengesList;
using YallaArabi.Application.Features.Admin.GetChallengeDetails;
using YallaArabi.Application.Features.Admin.AddChallengeQuestion;
using YallaArabi.Application.Features.Admin.DeleteChallengeQuestion;
using YallaArabi.Application.Features.Admin.GetCategories;
using YallaArabi.Application.Features.Admin.CreateCategory;
using YallaArabi.Application.Features.Admin.UpdateCategory;
using YallaArabi.Application.Features.Admin.DeleteCategory;
using YallaArabi.Application.Features.Admin.ForcePasswordReset;
using YallaArabi.Application.Features.Admin.UpdateChallengeQuestion;
using YallaArabi.Infrastructure.Storage;
using Microsoft.Extensions.Options;
using YallaArabi.Application.Features.Admin.GetChallengeParticipants;
using YallaArabi.Application.Features.Admin.AssignChallenge;

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

    [HttpGet("point-rules")]
    public async Task<IActionResult> GetPointRules(CancellationToken ct)
    {
        var result = await _sender.Send(new GetPointRulesQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<PointRuleItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<PointRuleItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("point-rules")]
    public async Task<IActionResult> AddPointRule(AddPointRuleCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AddPointRuleResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AddPointRuleResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPut("point-rules/{ruleId}")]
    public async Task<IActionResult> UpdatePointRule(Guid ruleId, [FromBody] UpdatePointRuleBody body, CancellationToken ct)
    {
        var command = new UpdatePointRuleCommand(ruleId, body.DisplayName, body.Description, body.Points, body.IsActive);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<UpdatePointRuleResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<UpdatePointRuleResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("challenges")]
    public async Task<IActionResult> GetChallenges(
        [FromQuery] string? status,
        [FromQuery] string? levelCode,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetChallengesListQuery(status, levelCode, pageNumber, pageSize), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<PagedChallengesResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<PagedChallengesResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("challenges/{challengeId}")]
    public async Task<IActionResult> GetChallengeDetails(Guid challengeId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetChallengeDetailsQuery(challengeId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<ChallengeDetailsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<ChallengeDetailsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("challenges")]
    public async Task<IActionResult> CreateChallenge(CreateChallengeCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CreateChallengeResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CreateChallengeResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPut("challenges/{challengeId}")]
    public async Task<IActionResult> UpdateChallenge(Guid challengeId, [FromBody] UpdateChallengeBody body, CancellationToken ct)
    {
        var command = new UpdateChallengeCommand(
            challengeId,
            body.Title,
            body.Description,
            body.LevelCode,
            body.PointsReward,
            body.TimeLimitSeconds,
            body.ExpiresAt);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Challenge updated successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("challenges/{challengeId}/publish")]
    public async Task<IActionResult> PublishChallenge(Guid challengeId, CancellationToken ct)
    {
        var result = await _sender.Send(new PublishChallengeCommand(challengeId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Challenge published successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpDelete("challenges/{challengeId}")]
    public async Task<IActionResult> DeleteChallenge(Guid challengeId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteChallengeCommand(challengeId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Challenge deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("challenges/{challengeId}/questions")]
    public async Task<IActionResult> AddChallengeQuestion(Guid challengeId, [FromBody] AddQuestionBody body, CancellationToken ct)
    {
        var command = new AddChallengeQuestionCommand(
            challengeId,
            body.QuestionText,
            body.OptionA,
            body.OptionB,
            body.OptionC,
            body.OptionD,
            body.CorrectOption,
            body.AudioUrl,
            body.ImageUrl);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AddChallengeQuestionResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AddChallengeQuestionResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpDelete("challenges/questions/{questionId}")]
    public async Task<IActionResult> DeleteChallengeQuestion(Guid questionId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteChallengeQuestionCommand(questionId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Question deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("levels/{levelCode}/categories")]
    public async Task<IActionResult> GetCategories(string levelCode, CancellationToken ct)
    {
        var result = await _sender.Send(new GetCategoriesQuery(levelCode), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<AdminCategoryItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<AdminCategoryItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(CreateCategoryCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CreateCategoryResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CreateCategoryResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPut("categories/{categoryId}")]
    public async Task<IActionResult> UpdateCategory(Guid categoryId, [FromBody] UpdateCategoryBody body, CancellationToken ct)
    {
        var command = new UpdateCategoryCommand(
            categoryId,
            body.Name,
            body.Description,
            body.IconUrl,
            body.OrderIndex,
            body.LevelCode);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Category updated successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpDelete("categories/{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteCategoryCommand(categoryId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Category deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("users/force-password-reset")]
    public async Task<IActionResult> ForcePasswordReset(ForcePasswordResetCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<ForcePasswordResetResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<ForcePasswordResetResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("uploads")]
    [RequestSizeLimit(15_000_000)]
    public async Task<IActionResult> UploadFile(
    IFormFile file,
    [FromQuery] string type,
    [FromServices] IFileStorage fileStorage,
    [FromServices] IOptions<FileStorageOptions> storageOptions,
    CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.Fail("file_required", "Please select a file to upload."));
        }

        var normalizedType = (type ?? "").Trim().ToLowerInvariant();

        if (normalizedType is not ("image" or "audio"))
        {
            return BadRequest(ApiResponse<object>.Fail("invalid_type", "Type must be either 'image' or 'audio'."));
        }

        var options = storageOptions.Value;
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        var allowedImage = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var allowedAudio = new[] { ".mp3", ".m4a", ".wav", ".ogg", ".aac", ".webm" };

        if (normalizedType == "image")
        {
            if (!allowedImage.Contains(extension))
            {
                return BadRequest(ApiResponse<object>.Fail("invalid_extension",
                    $"Allowed image formats: {string.Join(", ", allowedImage)}"));
            }

            if (file.Length > options.MaxImageSizeBytes)
            {
                return BadRequest(ApiResponse<object>.Fail("file_too_large",
                    $"Image size cannot exceed {options.MaxImageSizeBytes / 1_000_000} MB."));
            }
        }
        else
        {
            if (!allowedAudio.Contains(extension))
            {
                return BadRequest(ApiResponse<object>.Fail("invalid_extension",
                    $"Allowed audio formats: {string.Join(", ", allowedAudio)}"));
            }

            if (file.Length > options.MaxAudioSizeBytes)
            {
                return BadRequest(ApiResponse<object>.Fail("file_too_large",
                    $"Audio size cannot exceed {options.MaxAudioSizeBytes / 1_000_000} MB."));
            }
        }

        var folder = normalizedType == "image" ? "images" : "audio";

        await using var stream = file.OpenReadStream();
        var result = await fileStorage.UploadAsync(stream, file.FileName, file.ContentType, folder, ct);

        return Ok(ApiResponse<FileUploadResult>.Ok(result));
    }

    [HttpPut("challenges/questions/{questionId}")]
    public async Task<IActionResult> UpdateChallengeQuestion(
    Guid questionId,
    [FromBody] UpdateQuestionBody body,
    CancellationToken ct)
    {
        var command = new UpdateChallengeQuestionCommand(
            questionId,
            body.QuestionText,
            body.OptionA,
            body.OptionB,
            body.OptionC,
            body.OptionD,
            body.CorrectOption,
            body.OrderIndex,
            body.AudioUrl,
            body.ImageUrl);

        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Question updated successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpGet("challenges/{challengeId}/participants")]
    public async Task<IActionResult> GetChallengeParticipants(Guid challengeId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetChallengeParticipantsQuery(challengeId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<ChallengeParticipantsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<ChallengeParticipantsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("challenges/{challengeId}/assign")]
    public async Task<IActionResult> AssignChallenge(
    Guid challengeId,
    [FromBody] AssignChallengeBody body,
    CancellationToken ct)
    {
        var command = new AssignChallengeCommand(challengeId, body.UserIds, body.SendNotification);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AssignChallengeResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AssignChallengeResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
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

public sealed record UpdatePointRuleBody(string DisplayName, string? Description, int Points, bool IsActive);
public sealed record UpdateChallengeBody(
    string Title,
    string? Description,
    string? LevelCode,
    int PointsReward,
    int TimeLimitSeconds,
    DateTime? ExpiresAt
);


public sealed record AddQuestionBody(
   string QuestionText,
   string OptionA,
   string OptionB,
   string? OptionC,
   string? OptionD,
   string CorrectOption,
   string? AudioUrl,
   string? ImageUrl
);

public sealed record UpdateCategoryBody(
    string Name,
    string? Description,
    string? IconUrl,
    int OrderIndex,
    string? LevelCode
);

public sealed record UpdateQuestionBody(
    string QuestionText,
    string OptionA,
    string OptionB,
    string? OptionC,
    string? OptionD,
    string CorrectOption,
    int OrderIndex,
    string? AudioUrl,
    string? ImageUrl
);

public sealed record AssignChallengeBody(
    IReadOnlyList<Guid>? UserIds,
    bool SendNotification
);