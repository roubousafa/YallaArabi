using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;

namespace YallaArabi.Api.Controllers;

// Public details the app and the server's own pages show: where to get help and the legal pages.
[ApiController]
[Route("api/v1/app")]
public class AppInfoController : ControllerBase
{
    [HttpGet("info")]
    public IActionResult GetInfo([FromServices] IConfiguration configuration)
    {
        var publicUrl = (configuration["App:PublicUrl"] ?? string.Empty).TrimEnd('/');
        var supportEmail = configuration["App:SupportEmail"];

        return Ok(ApiResponse<AppInfoResponse>.Ok(new AppInfoResponse(
            string.IsNullOrWhiteSpace(supportEmail) ? null : supportEmail,
            $"{publicUrl}/privacy.html",
            $"{publicUrl}/delete-account.html")));
    }
}

public sealed record AppInfoResponse(string? SupportEmail, string PrivacyPolicyUrl, string DeleteAccountUrl);
