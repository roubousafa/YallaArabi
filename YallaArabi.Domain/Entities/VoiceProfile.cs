using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class VoiceProfile : AggregateRoot
{
    public string DisplayName { get; private set; } = default!;
    public string Dialect { get; private set; } = default!;
    public string Gender { get; private set; } = default!;
    public string Source { get; private set; } = default!;
    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private VoiceProfile() { }

    public static VoiceProfile Create(
        string displayName,
        string dialect,
        string gender,
        string source,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name is required", nameof(displayName));

        if (!Dialects.IsSupported(dialect))
            throw new ArgumentException("Unsupported dialect", nameof(dialect));

        if (!VoiceGenders.IsSupported(gender))
            throw new ArgumentException("Unsupported gender", nameof(gender));

        return new VoiceProfile
        {
            Id = Guid.NewGuid(),
            DisplayName = displayName.Trim(),
            Dialect = Dialects.Normalize(dialect),
            Gender = VoiceGenders.Normalize(gender),
            Source = source,
            IsActive = true,
            Notes = notes?.Trim(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string displayName, string? notes)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            throw new ArgumentException("Display name is required", nameof(displayName));

        DisplayName = displayName.Trim();
        Notes = notes?.Trim();
    }

    public void Activate() => IsActive = true;

    public void Deactivate() => IsActive = false;
}