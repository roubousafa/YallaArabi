namespace YallaArabi.Infrastructure.Storage;

public class FileStorageOptions
{
    public string RootFolder { get; set; } = "uploads";
    public string PublicBaseUrl { get; set; } = "";
    public long MaxImageSizeBytes { get; set; } = 5_000_000;
    public long MaxAudioSizeBytes { get; set; } = 10_000_000;
}