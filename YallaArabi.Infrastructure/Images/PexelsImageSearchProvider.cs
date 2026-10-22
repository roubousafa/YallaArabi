using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Infrastructure.Images;

// Pexels terms: hotlinking their image URLs is expected, and each photo is shown with "Photo by X on Pexels".
public class PexelsImageSearchProvider : IImageSearchProvider
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PexelsImageSearchProvider> _logger;

    public PexelsImageSearchProvider(HttpClient httpClient, IOptions<ImageSearchOptions> options, ILogger<PexelsImageSearchProvider> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", options.Value.PexelsApiKey);
    }

    public string Name => ImageSearchOptions.PexelsProvider;
    public bool IsEnabled => true;

    public async Task<ImageSearchResult?> FindAsync(string query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return null;
        }

        var url = $"search?query={Uri.EscapeDataString(query.Trim())}&per_page=1&orientation=square&locale=en-US";
        var response = await _httpClient.GetFromJsonAsync<PexelsSearchResponse>(url, ct);
        var photo = response?.Photos?.FirstOrDefault();

        if (photo?.Src?.Medium is null)
        {
            _logger.LogInformation("Pexels found no photo for an AI dictionary entry");
            return null;
        }

        return new ImageSearchResult(
            photo.Src.Medium,
            AiImageSources.Pexels,
            $"Photo by {photo.Photographer} on Pexels",
            photo.Url ?? "https://www.pexels.com");
    }

    private sealed record PexelsSearchResponse([property: JsonPropertyName("photos")] List<PexelsPhoto>? Photos);

    private sealed record PexelsPhoto(
        [property: JsonPropertyName("url")] string? Url,
        [property: JsonPropertyName("photographer")] string? Photographer,
        [property: JsonPropertyName("src")] PexelsPhotoSources? Src);

    private sealed record PexelsPhotoSources([property: JsonPropertyName("medium")] string? Medium);
}
