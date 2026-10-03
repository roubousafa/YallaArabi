using Microsoft.AspNetCore.Http;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Services;

// Reads the student's IANA time zone (e.g. "Europe/Berlin") from the X-Time-Zone header,
// so "today" means the student's local day. Falls back to UTC when the header is missing or unknown.
public class HeaderTimeZoneProvider : IClientTimeZoneProvider
{
    public const string HeaderName = "X-Time-Zone";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public HeaderTimeZoneProvider(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public DateOnly GetToday() =>
        DateOnly.FromDateTime(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ResolveTimeZone()));

    private TimeZoneInfo ResolveTimeZone()
    {
        var timeZoneId = _httpContextAccessor.HttpContext?.Request.Headers[HeaderName].ToString();

        if (string.IsNullOrWhiteSpace(timeZoneId))
        {
            return TimeZoneInfo.Utc;
        }

        return TimeZoneInfo.TryFindSystemTimeZoneById(timeZoneId.Trim(), out var timeZone)
            ? timeZone
            : TimeZoneInfo.Utc;
    }
}
