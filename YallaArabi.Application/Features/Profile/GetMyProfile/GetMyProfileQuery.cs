using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Profile.GetMyProfile;

public sealed record GetMyProfileQuery : IRequest<Result<MyProfileResponse>>;

public sealed record MyProfileResponse(
    Guid UserId,
    string FullName,
    string Email,
    string? AvatarUrl,
    bool HasPassword,
    bool IsGoogleLinked,
    string? LevelCode,
    int? DailyGoalMinutes,
    ProfileDialect? Dialect,
    string? VoiceGender,
    bool IsPremium,
    DateTime CreatedAt,
    bool IsEmailVerified
);

public sealed record ProfileDialect(string Key, string NameAr, string NameEn);

public sealed class GetMyProfileQueryHandler : IRequestHandler<GetMyProfileQuery, Result<MyProfileResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetMyProfileQueryHandler(
        IUserRepository userRepository,
        ILevelRepository levelRepository,
        ISubscriptionRepository subscriptionRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _levelRepository = levelRepository;
        _subscriptionRepository = subscriptionRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<MyProfileResponse>> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<MyProfileResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<MyProfileResponse>.Failure("user_not_found", "User not found");
        }

        var level = user.CurrentLevelId.HasValue
            ? await _levelRepository.GetByIdAsync(user.CurrentLevelId.Value, cancellationToken)
            : null;

        var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);
        var dialect = Dialects.Get(user.PreferredDialect);

        var response = new MyProfileResponse(
            user.Id,
            user.FullName,
            user.Email.Value,
            user.AvatarUrl,
            user.PasswordHash is not null,
            user.GoogleId is not null,
            level?.Code,
            user.DailyGoalMinutes,
            dialect is null ? null : new ProfileDialect(dialect.Key, dialect.NameAr, dialect.NameEn),
            user.PreferredVoiceGender,
            subscription is not null && subscription.IsCurrentlyActive(),
            user.CreatedAt,
            user.IsEmailVerified);

        return Result<MyProfileResponse>.Success(response);
    }
}
