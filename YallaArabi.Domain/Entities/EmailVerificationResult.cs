namespace YallaArabi.Domain.Entities;

public enum EmailVerificationResult
{
    Verified,
    Invalid,
    Expired,
    TooManyAttempts
}
