namespace YallaArabi.Application.Interfaces;

public interface IFileStorage
{
    Task<FileUploadResult> UploadAsync(Stream fileStream, string fileName, string contentType, string folder, CancellationToken ct = default);
    Task<bool> DeleteAsync(string fileUrl, CancellationToken ct = default);
}

public sealed record FileUploadResult(string Url, string FileName, long SizeInBytes);