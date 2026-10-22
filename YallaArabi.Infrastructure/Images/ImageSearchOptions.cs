namespace YallaArabi.Infrastructure.Images;

// Bound from the "ImageSearch" configuration section. Without a Pexels key no photos are looked up.
public class ImageSearchOptions
{
    public const string PexelsProvider = "pexels";
    public const string NoProvider = "none";

    public string Provider { get; set; } = NoProvider;
    public string? PexelsApiKey { get; set; }
    public string PexelsBaseUrl { get; set; } = "https://api.pexels.com/v1/";

    public bool UsePexels =>
        string.Equals(Provider, PexelsProvider, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(PexelsApiKey);
}
