using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Storage;

public class LocalFileStorage : IFileStorage
{
    private readonly FileStorageOptions _options;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<LocalFileStorage> _logger;

    public LocalFileStorage(
        IOptions<FileStorageOptions> options,
        IWebHostEnvironment environment,
        ILogger<LocalFileStorage> logger)
    {
        _options = options.Value;
        _environment = environment;
        _logger = logger;
    }

    public async Task<FileUploadResult> UploadAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        string folder,
        CancellationToken ct = default)
    {
        var safeFolder = SanitizeFolder(folder);
        var extension = Path.GetExtension(fileName);
        var uniqueName = $"{Guid.NewGuid():N}{extension}";

        var webRoot = _environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
        var targetDirectory = Path.Combine(webRoot, _options.RootFolder, safeFolder);

        Directory.CreateDirectory(targetDirectory);

        var fullPath = Path.Combine(targetDirectory, uniqueName);

        await using (var target = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
        {
            await fileStream.CopyToAsync(target, ct);
        }

        var fileInfo = new FileInfo(fullPath);
        var relativeUrl = $"/{_options.RootFolder}/{safeFolder}/{uniqueName}";
        var publicUrl = string.IsNullOrWhiteSpace(_options.PublicBaseUrl)
            ? relativeUrl
            : $"{_options.PublicBaseUrl.TrimEnd('/')}{relativeUrl}";

        _logger.LogInformation("File uploaded: {Url} ({Size} bytes)", relativeUrl, fileInfo.Length);

        return new FileUploadResult(publicUrl, uniqueName, fileInfo.Length);
    }

    public Task<bool> DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return Task.FromResult(false);
            }

            var relativePath = fileUrl;

            if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl) && fileUrl.StartsWith(_options.PublicBaseUrl))
            {
                relativePath = fileUrl[_options.PublicBaseUrl.TrimEnd('/').Length..];
            }

            relativePath = relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);

            var webRoot = _environment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot");
            var fullPath = Path.Combine(webRoot, relativePath);

            var normalizedRoot = Path.GetFullPath(webRoot);
            var normalizedTarget = Path.GetFullPath(fullPath);

            if (!normalizedTarget.StartsWith(normalizedRoot, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Blocked attempt to delete a file outside the upload root: {Path}", fileUrl);
                return Task.FromResult(false);
            }

            if (!File.Exists(normalizedTarget))
            {
                return Task.FromResult(false);
            }

            File.Delete(normalizedTarget);
            _logger.LogInformation("File deleted: {Url}", fileUrl);

            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {Url}", fileUrl);
            return Task.FromResult(false);
        }
    }

    private static string SanitizeFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
        {
            return "misc";
        }

        var cleaned = new string(folder.Where(c => char.IsLetterOrDigit(c) || c == '-' || c == '_').ToArray());
        return string.IsNullOrWhiteSpace(cleaned) ? "misc" : cleaned.ToLowerInvariant();
    }
}