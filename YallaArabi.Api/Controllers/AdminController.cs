using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Features.Admin.AddWord;
using YallaArabi.Application.Features.Admin.BulkImportWords;

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

    [HttpPost("words")]
    public async Task<IActionResult> AddWord(AddWordCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AddWordResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AddWordResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("words/bulk-import")]
    [RequestSizeLimit(10 * 1024 * 1024)]
    public async Task<IActionResult> BulkImportWords(IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiResponse<object>.Fail("file_required", "Please upload a valid Excel file"));
        }

        await using var stream = file.OpenReadStream();
        var result = await _sender.Send(new BulkImportWordsCommand(stream), ct);

        return Ok(ApiResponse<BulkImportResult>.Ok(result));
    }
}