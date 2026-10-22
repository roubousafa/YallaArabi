using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using YallaArabi.Api.RateLimiting;
using YallaArabi.Api.Common;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Auth.ForgotPassword;
using YallaArabi.Application.Features.Auth.GoogleLogin;
using YallaArabi.Application.Features.Auth.Login;
using YallaArabi.Application.Features.Auth.Logout;
using YallaArabi.Application.Features.Auth.RefreshToken;
using YallaArabi.Application.Features.Auth.Register;
using YallaArabi.Application.Features.Auth.ResetPassword;
using YallaArabi.Application.Features.Notifications.RegisterFcmToken;
using YallaArabi.Application.Features.Profile.UpdateProfile;
using YallaArabi.Application.Features.Auth.DeleteAccount;
using YallaArabi.Application.Features.Auth.ChangePassword;
using YallaArabi.Application.Features.Auth.ChangeEmail;
using YallaArabi.Application.Features.Auth.VerifyEmail;
using YallaArabi.Application.Features.Profile.SetAvatar;
using Microsoft.Extensions.Options;
using YallaArabi.Infrastructure.Storage;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) => _sender = sender;

    [HttpPost("register")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> Register(RegisterUserCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Value!))
            : Conflict(ApiResponse<AuthResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("login")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> Login(LoginUserCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Value!))
            : Unauthorized(ApiResponse<AuthResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("google")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> GoogleLogin(LoginWithGoogleCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Value!))
            : Unauthorized(ApiResponse<AuthResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("refresh")]
    [EnableRateLimiting(RateLimitPolicies.Refresh)]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Value!))
            : Unauthorized(ApiResponse<AuthResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutCommand command, CancellationToken ct)
    {
        await _sender.Send(command, ct);

        return Ok(ApiResponse<object>.Ok(new { message = "Logged out successfully." }));
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting(RateLimitPolicies.Email)]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken ct)
    {
        await _sender.Send(command, ct);

        return Ok(ApiResponse<object>.Ok(new { message = "If this email is registered, a reset link has been sent." }));
    }

    [HttpPost("reset-password")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> ResetPassword(ResetPasswordCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Password has been reset successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile(UpdateProfileCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<UpdateProfileResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<UpdateProfileResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("fcm-token")]
    public async Task<IActionResult> RegisterFcmToken(RegisterFcmTokenCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "FCM token registered successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
    [Authorize]
    [HttpPost("verify-email")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> VerifyEmail(VerifyEmailCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<EmailVerificationResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<EmailVerificationResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("resend-verification")]
    [EnableRateLimiting(RateLimitPolicies.Email)]
    public async Task<IActionResult> ResendVerification(CancellationToken ct)
    {
        var result = await _sender.Send(new ResendEmailVerificationCommand(), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<EmailVerificationResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<EmailVerificationResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpDelete("account")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountBody? body, CancellationToken ct)
    {
        var result = await _sender.Send(new DeleteAccountCommand(body?.Password), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Account deleted successfully." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("change-password")]
    [EnableRateLimiting(RateLimitPolicies.Auth)]
    public async Task<IActionResult> ChangePassword(ChangePasswordCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<object>.Ok(new { message = "Password changed successfully. Please sign in again." }))
            : BadRequest(ApiResponse<object>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("change-email")]
    [EnableRateLimiting(RateLimitPolicies.Email)]
    public async Task<IActionResult> ChangeEmail(ChangeEmailCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<ChangeEmailResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<ChangeEmailResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpPost("profile/avatar")]
    [RequestSizeLimit(6_000_000)]
    public async Task<IActionResult> UploadAvatar(
        IFormFile file,
        [FromServices] IOptions<FileStorageOptions> storageOptions,
        CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(ApiResponse<AvatarResponse>.Fail("file_required", "Please select an image to upload."));
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest(ApiResponse<AvatarResponse>.Fail(
                "invalid_extension",
                $"Allowed image formats: {string.Join(", ", allowedExtensions)}"));
        }

        if (file.Length > storageOptions.Value.MaxImageSizeBytes)
        {
            return BadRequest(ApiResponse<AvatarResponse>.Fail(
                "file_too_large",
                $"Image must be smaller than {storageOptions.Value.MaxImageSizeBytes / 1_000_000} MB."));
        }

        await using var stream = file.OpenReadStream();
        var result = await _sender.Send(new SetAvatarCommand(stream, file.FileName, file.ContentType), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AvatarResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AvatarResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [Authorize]
    [HttpDelete("profile/avatar")]
    public async Task<IActionResult> RemoveAvatar(CancellationToken ct)
    {
        var result = await _sender.Send(new SetAvatarCommand(null, null, null), ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AvatarResponse>.Ok(result.Value!))
            : BadRequest(ApiResponse<AvatarResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }
}

public sealed record DeleteAccountBody(string? Password);