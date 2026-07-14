namespace YallaArabi.Application.Interfaces;

public interface IGoogleAuthValidator
{
    Task<GoogleUserInfo?> ValidateAsync(string idToken, CancellationToken ct = default);
}

public sealed record GoogleUserInfo(string GoogleId, string Email, string FullName);