using YallaArabi.Domain.Common;
using YallaArabi.Domain.Events;
using YallaArabi.Domain.ValueObjects;

namespace YallaArabi.Domain.Entities;

public sealed class User : AggregateRoot
{
    public string FullName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public string? PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string SubscriptionStatus { get; private set; } = "free";
    public string? PasswordResetTokenHash { get; private set; }
    public DateTime? PasswordResetTokenExpiresAt { get; private set; }
    public Guid? CurrentLevelId { get; private set; }
    public string? GoogleId { get; private set; }
    public int? DailyGoalMinutes { get; private set; }
    public string? LearningMotivation { get; private set; }


    private User() { }

    public static User Register(string fullName, string emailValue, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required", nameof(passwordHash));

        var email = Email.Create(emailValue);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow,
            SubscriptionStatus = "free"
        };

        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, email.Value));

        return user;
    }

    public static User RegisterWithGoogle(string fullName, string emailValue, string googleId)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));

        if (string.IsNullOrWhiteSpace(googleId))
            throw new ArgumentException("Google ID is required", nameof(googleId));

        var email = Email.Create(emailValue);

        var user = new User
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            Email = email,
            PasswordHash = null,
            GoogleId = googleId,
            CreatedAt = DateTime.UtcNow,
            SubscriptionStatus = "free"
        };

        user.RaiseDomainEvent(new UserRegisteredEvent(user.Id, email.Value));

        return user;
    }

    public void LinkGoogleAccount(string googleId)
    {
        if (string.IsNullOrWhiteSpace(googleId))
            throw new ArgumentException("Google ID is required", nameof(googleId));

        GoogleId = googleId;
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash is required", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }

    public void UpdateFullName(string newFullName)
    {
        if (string.IsNullOrWhiteSpace(newFullName))
            throw new ArgumentException("Full name cannot be empty", nameof(newFullName));

        if (newFullName.Trim().Length > 150)
            throw new ArgumentException("Full name is too long", nameof(newFullName));

        FullName = newFullName.Trim();
    }

    public void SetPasswordResetToken(string tokenHash, DateTime expiresAt)
    {
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash is required", nameof(tokenHash));

        PasswordResetTokenHash = tokenHash;
        PasswordResetTokenExpiresAt = expiresAt;
    }

    public bool IsPasswordResetTokenValid(string tokenHash)
    {
        return PasswordResetTokenHash == tokenHash
            && PasswordResetTokenExpiresAt.HasValue
            && DateTime.UtcNow < PasswordResetTokenExpiresAt.Value;
    }

    public void ClearPasswordResetToken()
    {
        PasswordResetTokenHash = null;
        PasswordResetTokenExpiresAt = null;
    }

    public void SetCurrentLevel(Guid levelId)
    {
        CurrentLevelId = levelId;
    }

    public void SetDailyGoal(int minutes)
    {
        if (minutes <= 0)
            throw new ArgumentException("Daily goal minutes must be positive", nameof(minutes));

        DailyGoalMinutes = minutes;
    }

    public void SetLearningMotivation(string motivation)
    {
        if (string.IsNullOrWhiteSpace(motivation))
            throw new ArgumentException("Learning motivation is required", nameof(motivation));

        LearningMotivation = motivation;
    }
}