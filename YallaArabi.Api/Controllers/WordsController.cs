using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Words.GetWordById;
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
    [HttpGet("{wordId}")]
    public async Task<IActionResult> GetById(Guid wordId, CancellationToken ct)
    {
        var result = await _sender.Send(new GetWordByIdQuery(wordId), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<WordSearchResult>.Ok(result.Value!))
            : BadRequest(ApiResponse<WordSearchResult>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}