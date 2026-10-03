namespace YallaArabi.Application.Interfaces;

public interface IActivityService
{
    Task RecordAsync(Guid userId, string activityType, Guid relatedEntityId, int durationSeconds, CancellationToken ct = default);
}
