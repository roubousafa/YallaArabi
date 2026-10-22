using System.Text.Json;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using YallaArabi.Api.Common;

namespace YallaArabi.Api.RateLimiting;

// Limits are per client IP. They stop password guessing, sign-up spam and flooding inboxes with
// reset emails, while staying far above what one real person does.
public static class RateLimitPolicies
{
    // Sign-in, sign-up, password and account changes.
    public const string Auth = "auth";

    // Token refresh runs in the background on every app start, so it gets more room.
    public const string Refresh = "refresh";

    // Anything that sends an email.
    public const string Email = "email";

    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = async (context, ct) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    context.HttpContext.Response.Headers.RetryAfter = ((int)retryAfter.TotalSeconds).ToString();
                }

                context.HttpContext.Response.ContentType = "application/json";
                var body = ApiResponse<object>.Fail("too_many_requests", "Too many requests. Please wait a moment and try again.");
                await context.HttpContext.Response.WriteAsync(
                    JsonSerializer.Serialize(body, new JsonSerializerOptions(JsonSerializerDefaults.Web)), ct);
            };

            // A loose ceiling on everything, so one client can't flood the server.
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(ClientKey(context), _ => Window(600, TimeSpan.FromMinutes(1))));

            options.AddPolicy(Auth, context =>
                RateLimitPartition.GetFixedWindowLimiter(ClientKey(context), _ => Window(10, TimeSpan.FromMinutes(1))));

            options.AddPolicy(Refresh, context =>
                RateLimitPartition.GetFixedWindowLimiter(ClientKey(context), _ => Window(30, TimeSpan.FromMinutes(1))));

            options.AddPolicy(Email, context =>
                RateLimitPartition.GetFixedWindowLimiter(ClientKey(context), _ => Window(5, TimeSpan.FromMinutes(15))));
        });

        return services;
    }

    private static string ClientKey(HttpContext context) =>
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

    private static FixedWindowRateLimiterOptions Window(int permits, TimeSpan window) => new()
    {
        PermitLimit = permits,
        Window = window,
        QueueLimit = 0,
        AutoReplenishment = true
    };
}
