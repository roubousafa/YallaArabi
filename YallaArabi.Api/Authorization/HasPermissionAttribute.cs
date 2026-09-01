using Microsoft.AspNetCore.Authorization;

namespace YallaArabi.Api.Authorization;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public const string PolicyPrefix = "PERMISSION_";

    public HasPermissionAttribute(string permission)
        : base(PolicyPrefix + permission)
    {
    }
}