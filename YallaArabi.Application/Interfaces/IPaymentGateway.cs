namespace YallaArabi.Application.Interfaces;

public interface IPaymentGateway
{
    Task<CheckoutSession> CreateCheckoutSessionAsync(Guid userId, string planCode, CancellationToken ct = default);
    bool VerifyWebhookSignature(string payload, string signature);
}

public sealed record CheckoutSession(string SessionId, string CheckoutUrl);