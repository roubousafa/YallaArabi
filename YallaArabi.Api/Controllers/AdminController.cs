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
using YallaArabi.Application.Features.Admin.GetTeamList;
using YallaArabi.Application.Features.Admin.CreateTeamMember;
using YallaArabi.Application.Features.Admin.UpdateTeamMember;
using YallaArabi.Application.Features.Admin.DeleteTeamMember;
using YallaArabi.Application.Features.Admin.UpdateStudent;
using YallaArabi.Application.Features.Admin.DeleteStudent;
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
using YallaArabi.Application.Features.Admin.GetPermissions;
using YallaArabi.Application.Features.Admin.GetRoles;
using YallaArabi.Application.Features.Admin.CreateRole;
using YallaArabi.Application.Features.Admin.UpdateRole;
using YallaArabi.Application.Features.Admin.DeleteRole;
using YallaArabi.Application.Features.Admin.AssignUserRole;
using YallaArabi.Api.Authorization;
using YallaArabi.Domain.Common;
using YallaArabi.Application.Features.Admin.GetAuditLogs;
using YallaArabi.Application.Features.Admin.ExportAuditLogs;
using YallaArabi.Application.Features.Admin.GetStudentsReport;
using YallaArabi.Application.Features.Admin.ExportStudentsReport;
using YallaArabi.Application.Features.Admin.GetChallengesReport;
using YallaArabi.Application.Features.Admin.ExportChallengesReport;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/admin")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly ISender _sender;

    public AdminController(ISender sender) => _sender = sender;

    [HasPermission(Permissions.Dashboard.View)]
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok(ApiResponse<object>.Ok(new { message = "You are an authenticated Admin! 🎉" }));
    }

    [HasPermission(Permissions.Words.View)]
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

    [HasPermission(Permissions.Words.Create)]
    [HttpPost("words")]
    public async Task<IActionResult> AddWord(AddWordCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AddWordResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AddWordResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Words.Edit)]
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

    [HasPermission(Permissions.Words.Delete)]
    [HttpDelete("words/{wordId}")]
    public async Task<IActionResult> DeleteWord(Guid wordId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteWordCommand(wordId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Word deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Words.Import)]
    [HttpGet("words/excel-template")]
    public IActionResult DownloadExcelTemplate([FromServices] IExcelTemplateGenerator generator)
    {
        var fileBytes = generator.GenerateWordsTemplate();

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "YallaArabi-Words-Template.xlsx");
    }

    [HasPermission(Permissions.Words.Import)]
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

    [HasPermission(Permissions.Levels.View)]
    [HttpGet("levels")]
    public async Task<IActionResult> GetLevels(CancellationToken ct)
    {
        var result = await _sender.Send(new GetLevelsListQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<AdminLevelItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<AdminLevelItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Levels.Create)]
    [HttpPost("levels")]
    public async Task<IActionResult> AddLevel(AddLevelCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AddLevelResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AddLevelResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Levels.Edit)]
    [HttpPut("levels/{levelId}")]
    public async Task<IActionResult> UpdateLevel(Guid levelId, [FromBody] UpdateLevelBody body, CancellationToken ct)
    {
        var command = new UpdateLevelCommand(levelId, body.Name, body.OrderIndex);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<UpdateLevelResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<UpdateLevelResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Levels.Delete)]
    [HttpDelete("levels/{levelId}")]
    public async Task<IActionResult> DeleteLevel(Guid levelId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteLevelCommand(levelId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Level deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Students.View)]
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

    [HasPermission(Permissions.Students.View)]
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetStudentDetails(Guid userId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetStudentDetailsQuery(userId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<StudentDetailsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<StudentDetailsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Students.Edit)]
    [HttpPut("students/{userId}")]
    public async Task<IActionResult> UpdateStudent(Guid userId, [FromBody] UpdateStudentBody body, CancellationToken ct)
    {
        var command = new UpdateStudentCommand(userId, body.FullName, body.LevelCode);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Student updated successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Students.Delete)]
    [HttpDelete("students/{userId}")]
    public async Task<IActionResult> DeleteStudent(Guid userId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteStudentCommand(userId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Student deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Users.View)]
    [HttpGet("team")]
    public async Task<IActionResult> GetTeam(
        [FromQuery] string? searchTerm,
        [FromQuery] Guid? roleId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new GetTeamListQuery(searchTerm, roleId, pageNumber, pageSize), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<PagedTeamResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<PagedTeamResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Users.Create)]
    [HttpPost("team")]
    public async Task<IActionResult> CreateTeamMember(CreateTeamMemberCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CreateTeamMemberResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CreateTeamMemberResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Users.Edit)]
    [HttpPut("team/{userId}")]
    public async Task<IActionResult> UpdateTeamMember(Guid userId, [FromBody] UpdateTeamMemberBody body, CancellationToken ct)
    {
        var command = new UpdateTeamMemberCommand(userId, body.FullName, body.RoleId);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Team member updated successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Users.Delete)]
    [HttpDelete("team/{userId}")]
    public async Task<IActionResult> DeleteTeamMember(Guid userId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteTeamMemberCommand(userId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Team member deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Subscriptions.View)]
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

    [HasPermission(Permissions.Subscriptions.Grant)]
    [HttpPost("subscriptions/grant")]
    public async Task<IActionResult> GrantSubscription(GrantSubscriptionCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<GrantSubscriptionResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<GrantSubscriptionResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Subscriptions.Cancel)]
    [HttpPost("subscriptions/{subscriptionId}/cancel")]
    public async Task<IActionResult> CancelSubscription(Guid subscriptionId, CancellationToken ct)
    {
        var result = await _sender.Send(new CancelSubscriptionCommand(subscriptionId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Subscription cancelled successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Notifications.View)]
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

    [HasPermission(Permissions.Notifications.Send)]
    [HttpPost("notifications/send")]
    public async Task<IActionResult> SendNotification(SendCustomNotificationCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SendCustomNotificationResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SendCustomNotificationResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.PointRules.View)]
    [HttpGet("point-rules")]
    public async Task<IActionResult> GetPointRules(CancellationToken ct)
    {
        var result = await _sender.Send(new GetPointRulesQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<PointRuleItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<PointRuleItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.PointRules.Create)]
    [HttpPost("point-rules")]
    public async Task<IActionResult> AddPointRule(AddPointRuleCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AddPointRuleResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AddPointRuleResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.PointRules.Edit)]
    [HttpPut("point-rules/{ruleId}")]
    public async Task<IActionResult> UpdatePointRule(Guid ruleId, [FromBody] UpdatePointRuleBody body, CancellationToken ct)
    {
        var command = new UpdatePointRuleCommand(ruleId, body.DisplayName, body.Description, body.Points, body.IsActive);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<UpdatePointRuleResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<UpdatePointRuleResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Challenges.View)]
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

    [HasPermission(Permissions.Challenges.View)]
    [HttpGet("challenges/{challengeId}")]
    public async Task<IActionResult> GetChallengeDetails(Guid challengeId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetChallengeDetailsQuery(challengeId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<ChallengeDetailsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<ChallengeDetailsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Challenges.Create)]
    [HttpPost("challenges")]
    public async Task<IActionResult> CreateChallenge(CreateChallengeCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CreateChallengeResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CreateChallengeResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Challenges.Edit)]
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

    [HasPermission(Permissions.Challenges.Publish)]
    [HttpPost("challenges/{challengeId}/publish")]
    public async Task<IActionResult> PublishChallenge(Guid challengeId, CancellationToken ct)
    {
        var result = await _sender.Send(new PublishChallengeCommand(challengeId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Challenge published successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Challenges.Delete)]
    [HttpDelete("challenges/{challengeId}")]
    public async Task<IActionResult> DeleteChallenge(Guid challengeId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteChallengeCommand(challengeId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Challenge deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Challenges.Edit)]
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

    [HasPermission(Permissions.Challenges.Edit)]
    [HttpDelete("challenges/questions/{questionId}")]
    public async Task<IActionResult> DeleteChallengeQuestion(Guid questionId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteChallengeQuestionCommand(questionId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Question deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Categories.View)]
    [HttpGet("levels/{levelCode}/categories")]
    public async Task<IActionResult> GetCategories(string levelCode, CancellationToken ct)
    {
        var result = await _sender.Send(new GetCategoriesQuery(levelCode), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<AdminCategoryItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<AdminCategoryItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Categories.Create)]
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(CreateCategoryCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CreateCategoryResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CreateCategoryResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Categories.Edit)]
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

    [HasPermission(Permissions.Categories.Delete)]
    [HttpDelete("categories/{categoryId}")]
    public async Task<IActionResult> DeleteCategory(Guid categoryId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteCategoryCommand(categoryId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Category deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Students.ForcePasswordReset)]
    [HttpPost("users/force-password-reset")]
    public async Task<IActionResult> ForcePasswordReset(ForcePasswordResetCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<ForcePasswordResetResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<ForcePasswordResetResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Uploads.Upload)]
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

    [HasPermission(Permissions.Challenges.Edit)]
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

    [HasPermission(Permissions.Challenges.ViewParticipants)]
    [HttpGet("challenges/{challengeId}/participants")]
    public async Task<IActionResult> GetChallengeParticipants(Guid challengeId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetChallengeParticipantsQuery(challengeId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<ChallengeParticipantsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<ChallengeParticipantsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Challenges.Assign)]
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

    [HasPermission(Permissions.Roles.View)]
    [HttpGet("permissions")]
    public async Task<IActionResult> GetPermissions(CancellationToken ct)
    {
        var result = await _sender.Send(new GetPermissionsQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<PermissionGroupItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<PermissionGroupItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Roles.View)]
    [HttpGet("roles")]
    public async Task<IActionResult> GetRoles(CancellationToken ct)
    {
        var result = await _sender.Send(new GetRolesQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<RoleItem>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<RoleItem>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Roles.Create)]
    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole(CreateRoleCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<CreateRoleResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<CreateRoleResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Roles.Edit)]
    [HttpPut("roles/{roleId}")]
    public async Task<IActionResult> UpdateRole(Guid roleId, [FromBody] UpdateRoleBody body, CancellationToken ct)
    {
        var command = new UpdateRoleCommand(roleId, body.NameEn, body.NameAr, body.Description, body.Permissions);
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Role updated successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Roles.Delete)]
    [HttpDelete("roles/{roleId}")]
    public async Task<IActionResult> DeleteRole(Guid roleId, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteRoleCommand(roleId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Role deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Roles.Assign)]
    [HttpPut("users/{userId}/role")]
    public async Task<IActionResult> AssignUserRole(Guid userId, [FromBody] AssignRoleBody body, CancellationToken ct)
    {
        var result = await _sender.Send(new AssignUserRoleCommand(userId, body.RoleId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "User role updated successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
    [HasPermission(Permissions.AuditLogs.View)]
    [HttpGet("audit-logs")]
    public async Task<IActionResult> GetAuditLogs(
    [FromQuery] Guid? userId,
    [FromQuery] string? action,
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken ct = default)
    {
        var query = new GetAuditLogsQuery(userId, action, fromDate, toDate, pageNumber, pageSize);
        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<PagedAuditLogsResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<PagedAuditLogsResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.AuditLogs.Export)]
    [HttpGet("audit-logs/export")]
    public async Task<IActionResult> ExportAuditLogs(
        [FromQuery] Guid? userId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken ct = default)
    {
        var result = await _sender.Send(new ExportAuditLogsQuery(userId, fromDate, toDate), ct);

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
        }

        return File(result.Value!.Content, "text/csv", result.Value.FileName);
    }

    [HasPermission(Permissions.Reports.View)]
    [HttpGet("reports/students")]
    public async Task<IActionResult> GetStudentsReport(
    [FromQuery] string? searchTerm,
    [FromQuery] string? levelCode,
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 20,
    CancellationToken ct = default)
    {
        var query = new GetStudentsReportQuery(searchTerm, levelCode, fromDate, toDate, pageNumber, pageSize);
        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<PagedStudentsReportResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<PagedStudentsReportResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Reports.Export)]
    [HttpGet("reports/students/export")]
    public async Task<IActionResult> ExportStudentsReport(
        [FromQuery] string? searchTerm,
        [FromQuery] string? levelCode,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        CancellationToken ct = default)
    {
        var query = new ExportStudentsReportQuery(searchTerm, levelCode, fromDate, toDate);
        var result = await _sender.Send(query, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
        }

        return File(result.Value!.Content, "text/csv", result.Value.FileName);
    }

    [HasPermission(Permissions.Reports.View)]
    [HttpGet("reports/challenges")]
    public async Task<IActionResult> GetChallengesReport(
        [FromQuery] Guid? userId,
        [FromQuery] string? levelCode,
        [FromQuery] Guid? challengeId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var query = new GetChallengesReportQuery(userId, levelCode, challengeId, pageNumber, pageSize);
        var result = await _sender.Send(query, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<PagedChallengesReportResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<PagedChallengesReportResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HasPermission(Permissions.Reports.Export)]
    [HttpGet("reports/challenges/export")]
    public async Task<IActionResult> ExportChallengesReport(
        [FromQuery] Guid? userId,
        [FromQuery] string? levelCode,
        [FromQuery] Guid? challengeId,
        CancellationToken ct = default)
    {
        var query = new ExportChallengesReportQuery(userId, levelCode, challengeId);
        var result = await _sender.Send(query, ct);

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
        }

        return File(result.Value!.Content, "text/csv", result.Value.FileName);
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

public sealed record UpdateStudentBody(string FullName, string? LevelCode);

public sealed record UpdateTeamMemberBody(string FullName, Guid RoleId); 

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

public sealed record UpdateRoleBody(
    string NameEn,
    string NameAr,
    string? Description,
    IReadOnlyList<string> Permissions
);

public sealed record AssignRoleBody(Guid? RoleId);