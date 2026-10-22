using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using YallaArabi.Api.Common;

namespace YallaArabi.Api.Authorization;

// Blocks students who haven't confirmed their email, once Auth:RequireEmailVerification is turned on.
// Off by default so it can be switched on only after the app has the code screen.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class RequireVerifiedEmailAttribute : Attribute, IAuthorizationFilter
{
    private const string EmailVerifiedClaim = "email_verified";

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

        if (!configuration.GetValue<bool>("Auth:RequireEmailVerification"))
        {
            return;
        }

        // Tokens issued before this claim existed are let through; they expire within minutes.
        var claim = context.HttpContext.User.FindFirst(EmailVerifiedClaim)?.Value;

        if (claim == "false")
        {
            context.Result = new ObjectResult(ApiResponse<object>.Fail(
                "email_not_verified", "Please confirm your email with the code we sent you."))
            {
                StatusCode = StatusCodes.Status403Forbidden
            };
        }
    }
}
