using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YallaArabi.Api.Common;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Auth.ForgotPassword;
using YallaArabi.Application.Features.Auth.GoogleLogin;
using YallaArabi.Application.Features.Auth.Login;
using YallaArabi.Application.Features.Auth.Logout;
using YallaArabi.Application.Features.Auth.RefreshToken;
using YallaArabi.Application.Features.Auth.Register;
using YallaArabi.Application.Features.Auth.ResetPassword;
using YallaArabi.Application.Features.Profile.UpdateProfile;

namespace YallaArabi.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender) => _sender = sender;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Value!))
            : Conflict(ApiResponse<AuthResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Value!))
            : Unauthorized(ApiResponse<AuthResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin(LoginWithGoogleCommand command, CancellationToken ct)
    {
        var result = await _sender.Send(command, ct);

        return result.IsSuccess
            ? Ok(ApiResponse<AuthResponse>.Ok(result.Value!))
            : Unauthorized(ApiResponse<AuthResponse>.Fail(result.ErrorCode!, result.ErrorMessage!));
    }

    [HttpPost("refresh")]
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
    public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command, CancellationToken ct)
    {
        await _sender.Send(command, ct);

        return Ok(ApiResponse<object>.Ok(new { message = "If this email is registered, a reset link has been sent." }));
    }

    [HttpPost("reset-password")]
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
}