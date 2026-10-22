using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

// Single row holding the admin-controlled AI Mode switches.
public sealed class AiModeSettings : Entity
{
    public static readonly Guid SingletonId = new("a1a1a1a1-0001-0001-0001-000000000001");

    public const int DefaultDailyLimit = 20;

    public string Access { get; private set; } = AiModeAccess.AllStudents;

    // New AI generations allowed per student per day; 0 means no limit. Cached results never count.
    public int DailyLimit { get; private set; } = DefaultDailyLimit;

    public DateTime UpdatedAt { get; private set; }
    public Guid? UpdatedByUserId { get; private set; }

    private AiModeSettings() { }

    public static AiModeSettings CreateDefault() => new()
    {
        Id = SingletonId,
        Access = AiModeAccess.AllStudents,
        DailyLimit = DefaultDailyLimit,
        UpdatedAt = DateTime.UtcNow
    };

    public void Update(string access, int dailyLimit, Guid updatedByUserId)
    {
        if (!AiModeAccess.IsSupported(access))
            throw new ArgumentException("Unsupported access mode", nameof(access));

        if (dailyLimit < 0)
            throw new ArgumentException("Daily limit cannot be negative", nameof(dailyLimit));

        Access = access.Trim().ToLowerInvariant();
        DailyLimit = dailyLimit;
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = DateTime.UtcNow;
    }
}
