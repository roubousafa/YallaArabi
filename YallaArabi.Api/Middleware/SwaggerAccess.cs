using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace YallaArabi.Api.Middleware;

// Swagger is open in Development. On a real server it is off unless Swagger:Username and
// Swagger:Password are set, and then the browser asks for them before showing the page.
public static class SwaggerAccess
{
    public static bool IsEnabled(IWebHostEnvironment environment, IConfiguration configuration) =>
        environment.IsDevelopment() || HasCredentials(configuration);

    public static IApplicationBuilder UseSwaggerBasicAuth(this IApplicationBuilder app, IWebHostEnvironment environment, IConfiguration configuration)
    {
        if (environment.IsDevelopment())
        {
            return app;
        }

        var username = configuration["Swagger:Username"]!;
        var password = configuration["Swagger:Password"]!;

        return app.Use(async (context, next) =>
        {
            if (!context.Request.Path.StartsWithSegments("/swagger"))
            {
                await next();
                return;
            }

            if (IsAuthorized(context.Request.Headers.Authorization, username, password))
            {
                await next();
                return;
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.Headers.WWWAuthenticate = "Basic realm=\"YallaArabi API docs\"";
        });
    }

    private static bool HasCredentials(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration["Swagger:Username"]) &&
        !string.IsNullOrWhiteSpace(configuration["Swagger:Password"]);

    private static bool IsAuthorized(string? header, string username, string password)
    {
        if (!AuthenticationHeaderValue.TryParse(header, out var value) ||
            !string.Equals(value.Scheme, "Basic", StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrEmpty(value.Parameter))
        {
            return false;
        }

        string decoded;

        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(value.Parameter));
        }
        catch (FormatException)
        {
            return false;
        }

        var separator = decoded.IndexOf(':');

        if (separator < 0)
        {
            return false;
        }

        return FixedTimeEquals(decoded[..separator], username) && FixedTimeEquals(decoded[(separator + 1)..], password);
    }

    private static bool FixedTimeEquals(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(a), Encoding.UTF8.GetBytes(b));
}
