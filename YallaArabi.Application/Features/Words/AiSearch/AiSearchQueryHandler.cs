using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;
using YallaArabi.Domain.Entities;
using YallaArabi.Domain.Services;

namespace YallaArabi.Application.Features.Words.AiSearch;

public sealed class AiSearchQueryHandler : IRequestHandler<AiSearchQuery, Result<AiSearchResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IAiModeSettingsRepository _settingsRepository;
    private readonly IAiDictionaryRepository _entryRepository;
    private readonly IDialectAiProvider _aiProvider;
    private readonly IImageSearchProvider _imageProvider;
    private readonly IClientTimeZoneProvider _timeZoneProvider;
    private readonly ICurrentUserService _currentUserService;

    public AiSearchQueryHandler(
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository,
        IAiModeSettingsRepository settingsRepository,
        IAiDictionaryRepository entryRepository,
        IDialectAiProvider aiProvider,
        IImageSearchProvider imageProvider,
        IClientTimeZoneProvider timeZoneProvider,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
        _settingsRepository = settingsRepository;
        _entryRepository = entryRepository;
        _aiProvider = aiProvider;
        _imageProvider = imageProvider;
        _timeZoneProvider = timeZoneProvider;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<AiSearchResponse>> Handle(AiSearchQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<AiSearchResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<AiSearchResponse>.Failure("user_not_found", "User not found");
        }

        var dialect = Dialects.Get(user.PreferredDialect) ?? Dialects.Get(Dialects.DefaultDialect)!;
        var dialectInfo = new AiSearchDialect(dialect.Key, dialect.NameAr, dialect.NameEn);

        AiSearchResponse Respond(string status, AiDictionaryEntry? entry = null, int? remaining = null) =>
            new(status, dialectInfo, entry is null ? null : ToResult(entry), remaining);

        var settings = await _settingsRepository.GetAsync(cancellationToken);

        if (settings.Access == AiModeAccess.Disabled)
        {
            return Result<AiSearchResponse>.Success(Respond(AiSearchStatuses.Disabled));
        }

        if (settings.Access == AiModeAccess.SubscribersOnly)
        {
            var subscription = await _subscriptionRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);

            if (subscription is null || !subscription.IsCurrentlyActive())
            {
                return Result<AiSearchResponse>.Success(Respond(AiSearchStatuses.SubscriptionRequired));
            }
        }

        var normalizedQuery = TextNormalizer.Normalize(request.Query);

        if (string.IsNullOrEmpty(normalizedQuery))
        {
            return Result<AiSearchResponse>.Success(Respond(AiSearchStatuses.NotUnderstood));
        }

        // Every student shares one generated answer per query and dialect, so a cached hit is free and unlimited.
        var cached = await _entryRepository.GetAsync(normalizedQuery, dialect.Key, cancellationToken);

        if (cached is not null)
        {
            return Result<AiSearchResponse>.Success(cached.IsVisibleToStudents
                ? Respond(AiSearchStatuses.Ready, cached)
                : Respond(AiSearchStatuses.NotUnderstood));
        }

        var today = _timeZoneProvider.GetToday();
        int? remaining = null;

        if (settings.DailyLimit > 0)
        {
            var usedToday = await _entryRepository.CountRequestedByUserOnDateAsync(user.Id, today, cancellationToken);

            if (usedToday >= settings.DailyLimit)
            {
                return Result<AiSearchResponse>.Success(Respond(AiSearchStatuses.DailyLimitReached, remaining: 0));
            }

            remaining = settings.DailyLimit - usedToday - 1;
        }

        DialectAiResult generated;

        try
        {
            generated = await _aiProvider.GenerateAsync(request.Query.Trim(), dialect, cancellationToken);
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            // The provider logs the failure; the student still gets the dictionary results.
            return Result<AiSearchResponse>.Success(Respond(AiSearchStatuses.Unavailable));
        }

        var entry = generated.IsValid && !string.IsNullOrWhiteSpace(generated.DialectText)
            ? AiDictionaryEntry.CreateGenerated(
                request.Query,
                normalizedQuery,
                dialect.Key,
                generated.DialectText,
                generated.EnglishMeaning ?? string.Empty,
                generated.Arabizi ?? string.Empty,
                _aiProvider.Name,
                user.Id,
                today)
            : AiDictionaryEntry.CreateInvalid(request.Query, normalizedQuery, dialect.Key, _aiProvider.Name, user.Id, today);

        if (entry.IsVisibleToStudents && !string.IsNullOrWhiteSpace(generated.ImageQuery))
        {
            await AttachImageAsync(entry, generated.ImageQuery, cancellationToken);
        }

        // Invalid answers are stored too, so repeating nonsense doesn't call the AI again and still counts toward the limit.
        var saved = await _entryRepository.AddOrGetExistingAsync(entry, cancellationToken);

        return Result<AiSearchResponse>.Success(saved.IsVisibleToStudents
            ? Respond(AiSearchStatuses.Ready, saved, remaining)
            : Respond(AiSearchStatuses.NotUnderstood, remaining: remaining));
    }

    private async Task AttachImageAsync(AiDictionaryEntry entry, string imageQuery, CancellationToken cancellationToken)
    {
        if (!_imageProvider.IsEnabled)
        {
            return;
        }

        try
        {
            var image = await _imageProvider.FindAsync(imageQuery, cancellationToken);

            if (image is not null)
            {
                entry.SetImage(image.Url, image.Source, image.Credit, image.CreditUrl);
            }
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            // A missing photo is fine; the admin can add one later from the review queue.
        }
    }

    private static AiWordResult ToResult(AiDictionaryEntry entry) =>
        new(
            entry.Id,
            entry.Query,
            entry.DialectText ?? string.Empty,
            entry.EnglishMeaning ?? string.Empty,
            entry.Arabizi ?? string.Empty,
            entry.Status == AiEntryStatuses.Approved,
            entry.ImageUrl is null ? null : new AiWordImage(entry.ImageUrl, entry.ImageCredit, entry.ImageCreditUrl));
}
