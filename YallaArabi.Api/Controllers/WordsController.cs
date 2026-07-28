using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Words.GetWordById;
using YallaArabi.Application.Features.Words.GetSavedWords;
using YallaArabi.Application.Features.Words.RemoveSavedWord;
using YallaArabi.Application.Features.Words.SaveWord;
using YallaArabi.Application.Features.Words.SearchWords;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/words")]
public class WordsController : ControllerBase
{
    private readonly ISender _sender;

    public WordsController(ISender sender) => _sender = sender;

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, CancellationToken ct)
    {
        var result = await _sender.Send(new SearchWordsQuery(query), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<WordSearchResult>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<WordSearchResult>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpGet("saved")]
    public async Task<IActionResult> GetSavedWords([FromServices] ISender subscriptionSender, CancellationToken ct)
    {
        var subscriptionResult = await subscriptionSender.Send(new YallaArabi.Application.Features.Subscriptions.GetSubscriptionStatus.GetSubscriptionStatusQuery(), ct);

        if (!subscriptionResult.IsSuccess || subscriptionResult.Value!.SubscriptionStatus != "premium")
        {
            return StatusCode(403, ApiResponse<object>.Fail("premium_required", "This feature requires a premium subscription."));
        }

        var result = await _sender.Send(new GetSavedWordsQuery(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<IReadOnlyList<WordSearchResult>>.Ok(result.Value!))
            : BadRequest(ApiResponse<IReadOnlyList<WordSearchResult>>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("{wordId}/save")]
    public async Task<IActionResult> SaveWord(Guid wordId, CancellationToken ct)
    {
        var result = await _sender.Send(new SaveWordCommand(wordId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<SaveWordResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<SaveWordResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpDelete("{wordId}/save")]
    public async Task<IActionResult> RemoveSavedWord(Guid wordId, CancellationToken ct)
    {
        await _sender.Send(new RemoveSavedWordCommand(wordId), ct);

        return Ok(ApiResponse<object>.Ok(new { message = "Word removed from your saved list." }));
    }

    [Authorize]
    [HttpGet("{wordId}")]
    public async Task<IActionResult> GetById(Guid wordId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetWordByIdQuery(wordId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<WordSearchResult>.Ok(result.Value!))
            : BadRequest(ApiResponse<WordSearchResult>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}