namespace YallaArabi.Application.Interfaces;

public interface IPaymentGateway
{
    // False while no real provider is configured; checkout and webhooks are refused.
    bool IsAvailable { get; }

    Task<CheckoutSession> CreateCheckoutSessionAsync(Guid userId, string planCode, CancellationToken ct = default);
    bool VerifyWebhookSignature(string payload, string signature);
}

public sealed record CheckoutSession(string SessionId, string CheckoutUrl);