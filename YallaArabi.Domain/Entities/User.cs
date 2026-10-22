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
    public string? PreferredDialect { get; private set; }
    public string? PreferredVoiceGender { get; private set; }
    public string? FcmToken { get; private set; }
    public string Role { get; private set; } = "Student";
    public Guid? RoleId { get; private set; }
    public bool MustChangePassword { get; private set; }

    public string? FriendCode { get; private set; }
    public string? AvatarUrl { get; private set; }
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockoutEndsAt { get; private set; }
    public DateTime? EmailVerifiedAt { get; private set; }
    public string? EmailVerificationCodeHash { get; private set; }
    public DateTime? EmailVerificationExpiresAt { get; private set; }
    public int EmailVerificationAttempts { get; private set; }

    public const int MaxEmailVerificationAttempts = 5;
    public static readonly TimeSpan EmailVerificationLifetime = TimeSpan.FromMinutes(30);

    public bool IsEmailVerified => EmailVerifiedAt.HasValue;

    public const int MaxFailedLoginAttempts = 5;
    public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
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
            EmailVerifiedAt = DateTime.UtcNow,
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

        // Signing in with Google proves the person owns this address.
        if (!IsEmailVerified)
        {
            MarkEmailVerified();
        }
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

    public void ChangeEmail(string newEmail)
    {
        Email = Email.Create(newEmail);
        EmailVerifiedAt = null;
        ClearEmailVerificationCode();
    }

    public void MarkEmailVerified()
    {
        EmailVerifiedAt = DateTime.UtcNow;
        ClearEmailVerificationCode();
    }

    public void SetEmailVerificationCode(string codeHash, DateTime utcNow)
    {
        if (string.IsNullOrWhiteSpace(codeHash))
            throw new ArgumentException("Code hash is required", nameof(codeHash));

        EmailVerificationCodeHash = codeHash;
        EmailVerificationExpiresAt = utcNow.Add(EmailVerificationLifetime);
        EmailVerificationAttempts = 0;
    }

    // When the current code was sent, derived from its expiry.
    public DateTime? EmailVerificationSentAt => EmailVerificationExpiresAt?.Subtract(EmailVerificationLifetime);

    public EmailVerificationResult VerifyEmailCode(string codeHash, DateTime utcNow)
    {
        if (IsEmailVerified)
            return EmailVerificationResult.Verified;

        if (EmailVerificationCodeHash is null || EmailVerificationExpiresAt is null || utcNow >= EmailVerificationExpiresAt)
            return EmailVerificationResult.Expired;

        if (EmailVerificationAttempts >= MaxEmailVerificationAttempts)
            return EmailVerificationResult.TooManyAttempts;

        if (EmailVerificationCodeHash != codeHash)
        {
            EmailVerificationAttempts++;
            return EmailVerificationAttempts >= MaxEmailVerificationAttempts
                ? EmailVerificationResult.TooManyAttempts
                : EmailVerificationResult.Invalid;
        }

        MarkEmailVerified();
        return EmailVerificationResult.Verified;
    }

    private void ClearEmailVerificationCode()
    {
        EmailVerificationCodeHash = null;
        EmailVerificationExpiresAt = null;
        EmailVerificationAttempts = 0;
    }

    public void SetAvatarUrl(string? avatarUrl)
    {
        AvatarUrl = string.IsNullOrWhiteSpace(avatarUrl) ? null : avatarUrl.Trim();
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

    public bool IsLockedOut(DateTime utcNow) => LockoutEndsAt.HasValue && utcNow < LockoutEndsAt.Value;

    // Locks password sign-in for a while after too many wrong passwords in a row.
    public void RegisterFailedLogin(DateTime utcNow)
    {
        FailedLoginAttempts++;

        if (FailedLoginAttempts >= MaxFailedLoginAttempts)
        {
            LockoutEndsAt = utcNow.Add(LockoutDuration);
            FailedLoginAttempts = 0;
        }
    }

    public void ClearFailedLogins()
    {
        FailedLoginAttempts = 0;
        LockoutEndsAt = null;
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

    public void SetPreferredDialect(string dialect)
    {
        if (string.IsNullOrWhiteSpace(dialect))
            throw new ArgumentException("Preferred dialect is required", nameof(dialect));

        PreferredDialect = dialect.Trim().ToLowerInvariant();
    }

    public void SetPreferredVoiceGender(string gender)
    {
        if (string.IsNullOrWhiteSpace(gender))
            throw new ArgumentException("Preferred voice gender is required", nameof(gender));

        PreferredVoiceGender = gender.Trim().ToLowerInvariant();
    }

    public void SetFcmToken(string fcmToken)
    {
        if (string.IsNullOrWhiteSpace(fcmToken))
            throw new ArgumentException("FCM token is required", nameof(fcmToken));

        FcmToken = fcmToken;
    }

    public void PromoteToAdmin()
    {
        Role = "Admin";
    }

    public void ActivatePremium()
    {
        SubscriptionStatus = "premium";
    }

    public void DeactivatePremium()
    {
        SubscriptionStatus = "free";
    }

    public void ChangeRole(string role)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role is required", nameof(role));

        Role = role;
    }

    public void UpdateByAdmin(string fullName, Guid? currentLevelId)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty", nameof(fullName));

        FullName = fullName.Trim();
        CurrentLevelId = currentLevelId;
    }
    public void RequirePasswordChange()
    {
        MustChangePassword = true;
    }

    public void ClearPasswordChangeRequirement()
    {
        MustChangePassword = false;
    }

    public void AssignRole(Guid roleId)
    {
        RoleId = roleId;
        Role = "Admin";
    }

    public void RemoveRole()
    {
        RoleId = null;
        Role = "Student";
    }

    public void SetFriendCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException("Friend code is required", nameof(code));

        FriendCode = code.Trim().ToUpperInvariant();
    }
}