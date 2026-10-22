namespace YallaArabi.Infrastructure.Links;

// Bound from the "App" configuration section.
public class AppLinksOptions
{
    // The server's public address, e.g. https://api.yallaarabi.com. Used for pages the server hosts itself.
    public string PublicUrl { get; set; } = string.Empty;

    // Optional: a page or app deep link that handles password resets. Defaults to the server's own reset page.
    public string? PasswordResetUrl { get; set; }
}
