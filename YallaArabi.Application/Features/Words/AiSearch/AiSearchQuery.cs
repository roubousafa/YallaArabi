using FluentValidation;
using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Words.AiSearch;

public sealed record AiSearchQuery(string Query) : IRequest<Result<AiSearchResponse>>;

public sealed record AiSearchResponse(
    string Status,
    AiSearchDialect Dialect,
    AiWordResult? Result,
    int? RemainingToday
);

public sealed record AiSearchDialect(string Key, string NameAr, string NameEn);

public sealed record AiWordResult(
    Guid EntryId,
    string Query,
    string DialectText,
    string EnglishMeaning,
    string Arabizi,
    bool IsVerified,
    AiWordImage? Image
);

// Credit and CreditUrl must be shown under the photo when present (required by Pexels).
public sealed record AiWordImage(string Url, string? Credit, string? CreditUrl);

// What the app shows under the dictionary results.
public static class AiSearchStatuses
{
    public const string Ready = "ready";
    public const string Disabled = "disabled";
    public const string SubscriptionRequired = "subscription_required";
    public const string DailyLimitReached = "daily_limit_reached";
    public const string NotUnderstood = "not_understood";
    public const string Unavailable = "unavailable";
}

public sealed class AiSearchQueryValidator : AbstractValidator<AiSearchQuery>
{
    public AiSearchQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty().WithMessage("Query is required")
            .MaximumLength(100).WithMessage("Query is too long");
    }
}
