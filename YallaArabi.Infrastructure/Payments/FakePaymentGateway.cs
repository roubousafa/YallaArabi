using Microsoft.Extensions.Logging;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Payments;

public class FakePaymentGateway : IPaymentGateway
{
    private readonly ILogger<FakePaymentGateway> _logger;

    public FakePaymentGateway(ILogger<FakePaymentGateway> logger) => _logger = logger;

    public Task<CheckoutSession> CreateCheckoutSessionAsync(Guid userId, string planCode, CancellationToken ct = default)
    {
        var fakeSessionId = $"fake_session_{Guid.NewGuid():N}";
        var fakeUrl = $"https://fake-checkout.local/pay?session={fakeSessionId}&plan={planCode}";

        _logger.LogInformation(
            "[DEV PAYMENT] Created fake checkout session {SessionId} for user {UserId}, plan {PlanCode}. In production, the user would be redirected to: {Url}",
            fakeSessionId, userId, planCode, fakeUrl);

        return Task.FromResult(new CheckoutSession(fakeSessionId, fakeUrl));
    }

    public bool VerifyWebhookSignature(string payload, string signature)
    {
        _logger.LogInformation("[DEV PAYMENT] Skipping signature verification (fake gateway).");
        return true;
    }
}