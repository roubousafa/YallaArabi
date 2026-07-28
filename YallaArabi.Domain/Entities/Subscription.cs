using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Subscription : AggregateRoot
{
    public Guid UserId { get; private set; }
    public string PlanCode { get; private set; } = default!;
    public string Status { get; private set; } = default!;
    public DateTime StartedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? PaymentProviderReference { get; private set; }

    private Subscription() { }

    public static Subscription Create(Guid userId, string planCode, string paymentProviderReference, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(planCode))
            throw new ArgumentException("Plan code is required", nameof(planCode));

        if (string.IsNullOrWhiteSpace(paymentProviderReference))
            throw new ArgumentException("Payment provider reference is required", nameof(paymentProviderReference));

        return new Subscription
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            PlanCode = planCode,
            Status = "active",
            StartedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            PaymentProviderReference = paymentProviderReference
        };
    }

    public void Cancel()
    {
        Status = "cancelled";
    }

    public bool IsCurrentlyActive() =>
        Status == "active" && (ExpiresAt is null || DateTime.UtcNow < ExpiresAt.Value);
}