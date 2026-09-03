using System.Security.Claims;
using YallaArabi.Api.Auditing;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Api.Middleware;

public class AuditLoggingMiddleware
{
    private static readonly string[] TrackedMethods = { "POST", "PUT", "DELETE", "PATCH" };

    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        await _next(context);

        try
        {
            if (!ShouldAudit(context))
            {
                return;
            }

            var userIdClaim = context.User.FindFirst("sub")?.Value
                              ?? context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return;
            }

            var fullName = context.User.FindFirst("full_name")?.Value ?? "-";
            var email = context.User.FindFirst("email")?.Value
                        ?? context.User.FindFirst(ClaimTypes.Email)?.Value
                        ?? "-";

            var method = context.Request.Method.ToUpperInvariant();
            var path = context.Request.Path.Value ?? "";

            var (actionEn, actionAr, entityType) = AuditActionMap.Resolve(method, path);
            var entityId = AuditActionMap.ExtractEntityId(path);
            var ip = context.Connection.RemoteIpAddress?.ToString();

            using var scope = serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();

            var log = AuditLog.Create(
                userId,
                fullName,
                email,
                actionEn,
                actionAr,
                method,
                path,
                context.Response.StatusCode,
                entityType,
                entityId,
                ip);

            await repository.AddAsync(log);
            await repository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write audit log entry");
        }
    }

    private static bool ShouldAudit(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";

        if (!path.Contains("/admin/"))
        {
            return false;
        }

        if (!TrackedMethods.Contains(context.Request.Method.ToUpperInvariant()))
        {
            return false;
        }

        var status = context.Response.StatusCode;

        return (status >= 200 && status < 300) || status == 403;
    }
}