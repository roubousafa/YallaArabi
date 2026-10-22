using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Payments;

// Stand-in until a real payment provider is wired in. It only works in Development: on a real server
// it would let anyone activate premium through the public webhook, so it refuses everything there.
public class FakePaymentGateway : IPaymentGateway
{
    private readonly ILogger<FakePaymentGateway> _logger;
    private readonly bool _isDevelopment;

    public FakePaymentGateway(ILogger<FakePaymentGateway> logger, IHostEnvironment environment)
    {
        _logger = logger;
        _isDevelopment = environment.IsDevelopment();
    }

    public bool IsAvailable => _isDevelopment;

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
        if (!_isDevelopment)
        {
            _logger.LogWarning("Rejected a payment webhook: no real payment provider is configured.");
            return false;
        }

        _logger.LogInformation("[DEV PAYMENT] Skipping signature verification (fake gateway).");
        return true;
    }
}
