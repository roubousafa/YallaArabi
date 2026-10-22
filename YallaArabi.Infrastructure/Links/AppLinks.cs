using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Links;

public class AppLinks : IAppLinks
{
    private const string ServerResetPage = "/reset-password.html";

    private readonly AppLinksOptions _options;
    private readonly ILogger<AppLinks> _logger;

    public AppLinks(IOptions<AppLinksOptions> options, ILogger<AppLinks> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public string PasswordReset(string token, string email)
    {
        var parameters = $"token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

        if (!string.IsNullOrWhiteSpace(_options.PasswordResetUrl))
        {
            var separator = _options.PasswordResetUrl.Contains('?') ? '&' : '?';
            return $"{_options.PasswordResetUrl}{separator}{parameters}";
        }

        if (string.IsNullOrWhiteSpace(_options.PublicUrl))
        {
            _logger.LogWarning("App:PublicUrl is not set, so password reset emails carry a link without the server address.");
        }

        // Our own page reads the values from the fragment, which browsers never send to the server or its logs.
        return $"{_options.PublicUrl.TrimEnd('/')}{ServerResetPage}#{parameters}";
    }
}
