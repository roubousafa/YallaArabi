using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Infrastructure.Authentication;

public class GoogleAuthValidator : IGoogleAuthValidator
{
    private readonly GoogleOptions _options;

    public GoogleAuthValidator(IOptions<GoogleOptions> options) => _options = options.Value;

    public async Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken ct = default)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _options.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

            return new GoogleUserInfo(payload.Subject, payload.Email, payload.Name);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }
}