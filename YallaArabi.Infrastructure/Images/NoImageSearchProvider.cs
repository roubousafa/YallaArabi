using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Images;

// Used until a Pexels key is configured; entries simply have no automatic photo.
public class NoImageSearchProvider : IImageSearchProvider
{
    public string Name => ImageSearchOptions.NoProvider;
    public bool IsEnabled => false;

    public Task<ImageSearchResult?> FindAsync(string query, CancellationToken ct = default) =>
        Task.FromResult<ImageSearchResult?>(null);
}
