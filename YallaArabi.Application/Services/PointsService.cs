using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Services;

public sealed class PointsService : IPointsService
{
    private readonly IPointRuleRepository _pointRuleRepository;
    private readonly IPointTransactionRepository _pointTransactionRepository;

    public PointsService(
        IPointRuleRepository pointRuleRepository,
        IPointTransactionRepository pointTransactionRepository)
    {
        _pointRuleRepository = pointRuleRepository;
        _pointTransactionRepository = pointTransactionRepository;
    }

    public async Task<int> AwardPointsAsync(
        Guid userId,
        string actionKey,
        Guid? relatedEntityId = null,
        string? note = null,
        CancellationToken ct = default)
    {
        var rule = await _pointRuleRepository.GetByActionKeyAsync(actionKey, ct);

        if (rule is null || !rule.IsActive || rule.Points <= 0)
        {
            return 0;
        }

        if (relatedEntityId.HasValue)
        {
            var alreadyAwarded = await _pointTransactionRepository.ExistsAsync(userId, actionKey, relatedEntityId.Value, ct);

            if (alreadyAwarded)
            {
                return 0;
            }
        }

        var transaction = PointTransaction.Create(userId, actionKey, rule.Points, relatedEntityId, note);

        await _pointTransactionRepository.AddAsync(transaction, ct);
        await _pointTransactionRepository.SaveChangesAsync(ct);

        return rule.Points;
    }

    public async Task<int> GetTotalPointsAsync(Guid userId, CancellationToken ct = default) =>
        await _pointTransactionRepository.GetTotalPointsAsync(userId, ct);
}