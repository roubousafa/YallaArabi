namespace YallaArabi.Application.Interfaces;

public interface IPointsService
{
    Task<int> AwardPointsAsync(Guid userId, string actionKey, Guid? relatedEntityId = null, string? note = null, CancellationToken ct = default);
    Task<int> GetTotalPointsAsync(Guid userId, CancellationToken ct = default);
}