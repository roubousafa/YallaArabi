using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using YallaArabi.Infrastructure.Security;

namespace YallaArabi.Api.Authorization;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private const string SuperAdminRoleClaimValue = "Admin";

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        var hasRoleId = context.User.HasClaim(c => c.Type == "role_id");

        if (!hasRoleId && context.User.IsInRole(SuperAdminRoleClaimValue))
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var hasPermission = context.User.HasClaim(
            JwtTokenGenerator.PermissionClaimType,
            requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}