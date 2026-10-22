using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

// One AI Mode answer per (normalized query, dialect), generated once and reused for every student.
public sealed class AiDictionaryEntry : Entity
{
    public string Query { get; private set; } = default!;
    public string NormalizedQuery { get; private set; } = default!;
    public string Dialect { get; private set; } = default!;
    public string? DialectText { get; private set; }
    public string? EnglishMeaning { get; private set; }
    public string? Arabizi { get; private set; }
    public string? ImageUrl { get; private set; }
    public string? ImageSource { get; private set; }
    public string? ImageCredit { get; private set; }
    public string? ImageCreditUrl { get; private set; }
    public string Status { get; private set; } = default!;
    public string Provider { get; private set; } = default!;
    public Guid? RequestedByUserId { get; private set; }
    public DateOnly RequestedLocalDate { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public Guid? ReviewedByUserId { get; private set; }

    private AiDictionaryEntry() { }

    public static AiDictionaryEntry CreateGenerated(
        string query,
        string normalizedQuery,
        string dialect,
        string dialectText,
        string englishMeaning,
        string arabizi,
        string provider,
        Guid requestedByUserId,
        DateOnly requestedLocalDate)
    {
        if (string.IsNullOrWhiteSpace(dialectText))
            throw new ArgumentException("Dialect text is required", nameof(dialectText));

        var entry = Create(query, normalizedQuery, dialect, AiEntryStatuses.Generated, provider, requestedByUserId, requestedLocalDate);
        entry.DialectText = dialectText.Trim();
        entry.EnglishMeaning = englishMeaning.Trim();
        entry.Arabizi = arabizi.Trim();
        return entry;
    }

    public static AiDictionaryEntry CreateInvalid(
        string query,
        string normalizedQuery,
        string dialect,
        string provider,
        Guid requestedByUserId,
        DateOnly requestedLocalDate) =>
        Create(query, normalizedQuery, dialect, AiEntryStatuses.Invalid, provider, requestedByUserId, requestedLocalDate);

    private static AiDictionaryEntry Create(
        string query,
        string normalizedQuery,
        string dialect,
        string status,
        string provider,
        Guid requestedByUserId,
        DateOnly requestedLocalDate)
    {
        if (string.IsNullOrWhiteSpace(normalizedQuery))
            throw new ArgumentException("Normalized query is required", nameof(normalizedQuery));

        return new AiDictionaryEntry
        {
            Id = Guid.NewGuid(),
            Query = query.Trim(),
            NormalizedQuery = normalizedQuery,
            Dialect = dialect,
            Status = status,
            Provider = provider,
            RequestedByUserId = requestedByUserId,
            RequestedLocalDate = requestedLocalDate,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool IsVisibleToStudents => Status is AiEntryStatuses.Generated or AiEntryStatuses.Approved;

    // An admin edit is a review, so the corrected entry is approved in the same step.
    public void EditAndApprove(string dialectText, string englishMeaning, string arabizi, Guid adminId)
    {
        if (string.IsNullOrWhiteSpace(dialectText))
            throw new ArgumentException("Dialect text is required", nameof(dialectText));

        DialectText = dialectText.Trim();
        EnglishMeaning = englishMeaning.Trim();
        Arabizi = arabizi.Trim();
        MarkReviewed(AiEntryStatuses.Approved, adminId);
    }

    public void Approve(Guid adminId)
    {
        if (string.IsNullOrWhiteSpace(DialectText))
            throw new InvalidOperationException("An entry without text must be edited before it can be approved");

        MarkReviewed(AiEntryStatuses.Approved, adminId);
    }

    public void Reject(Guid adminId) => MarkReviewed(AiEntryStatuses.Rejected, adminId);

    public void ForgetRequester() => RequestedByUserId = null;

    public void SetImage(string url, string source, string? credit, string? creditUrl)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Image URL is required", nameof(url));

        ImageUrl = url.Trim();
        ImageSource = source;
        ImageCredit = string.IsNullOrWhiteSpace(credit) ? null : credit.Trim();
        ImageCreditUrl = string.IsNullOrWhiteSpace(creditUrl) ? null : creditUrl.Trim();
    }

    public void RemoveImage()
    {
        ImageUrl = null;
        ImageSource = null;
        ImageCredit = null;
        ImageCreditUrl = null;
    }

    private void MarkReviewed(string status, Guid adminId)
    {
        Status = status;
        ReviewedByUserId = adminId;
        ReviewedAt = DateTime.UtcNow;
    }
}
