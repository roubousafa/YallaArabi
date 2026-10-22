namespace YallaArabi.Application.Interfaces;

// Finds a free photo that illustrates a word; returns null when nothing fits or no provider is configured.
public interface IImageSearchProvider
{
    string Name { get; }
    bool IsEnabled { get; }
    Task<ImageSearchResult?> FindAsync(string query, CancellationToken ct = default);
}

public sealed record ImageSearchResult(string Url, string Source, string Credit, string CreditUrl);
