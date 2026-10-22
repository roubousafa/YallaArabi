using FluentValidation;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.AiMode;

public sealed record AiEntryItem(
    Guid Id,
    string Query,
    string Dialect,
    string? DialectText,
    string? EnglishMeaning,
    string? Arabizi,
    string? ImageUrl,
    string? ImageSource,
    string? ImageCredit,
    string? ImageCreditUrl,
    string Status,
    string Provider,
    DateTime CreatedAt,
    DateTime? ReviewedAt
);

public sealed record PagedAiEntriesResponse(
    IReadOnlyList<AiEntryItem> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages
);

internal static class AiEntryMapping
{
    public static AiEntryItem ToItem(AiDictionaryEntry e) =>
        new(e.Id, e.Query, e.Dialect, e.DialectText, e.EnglishMeaning, e.Arabizi,
            e.ImageUrl, e.ImageSource, e.ImageCredit, e.ImageCreditUrl,
            e.Status, e.Provider, e.CreatedAt, e.ReviewedAt);
}

public sealed record GetAiEntriesQuery(string? Status, string? Dialect, int PageNumber = 1, int PageSize = 20)
    : IRequest<Result<PagedAiEntriesResponse>>;

public sealed class GetAiEntriesQueryHandler : IRequestHandler<GetAiEntriesQuery, Result<PagedAiEntriesResponse>>
{
    private const int MaxPageSize = 100;
    private readonly IAiDictionaryRepository _entryRepository;

    public GetAiEntriesQueryHandler(IAiDictionaryRepository entryRepository) => _entryRepository = entryRepository;

    public async ValueTask<Result<PagedAiEntriesResponse>> Handle(GetAiEntriesQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = Math.Max(request.PageNumber, 1);
        var pageSize = Math.Clamp(request.PageSize, 1, MaxPageSize);

        var (items, total) = await _entryRepository.GetPagedAsync(
            request.Status?.Trim().ToLowerInvariant(),
            request.Dialect?.Trim().ToLowerInvariant(),
            pageNumber,
            pageSize,
            cancellationToken);

        return Result<PagedAiEntriesResponse>.Success(new PagedAiEntriesResponse(
            items.Select(AiEntryMapping.ToItem).ToList(),
            total,
            pageNumber,
            pageSize,
            (int)Math.Ceiling(total / (double)pageSize)));
    }
}

public enum AiEntryReviewAction
{
    Approve,
    Reject
}

public sealed record ReviewAiEntryCommand(Guid EntryId, AiEntryReviewAction Action) : IRequest<Result<AiEntryItem>>;

public sealed class ReviewAiEntryCommandHandler : IRequestHandler<ReviewAiEntryCommand, Result<AiEntryItem>>
{
    private readonly IAiDictionaryRepository _entryRepository;
    private readonly ICurrentUserService _currentUserService;

    public ReviewAiEntryCommandHandler(IAiDictionaryRepository entryRepository, ICurrentUserService currentUserService)
    {
        _entryRepository = entryRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<AiEntryItem>> Handle(ReviewAiEntryCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId;

        if (adminId is null)
        {
            return Result<AiEntryItem>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var entry = await _entryRepository.GetByIdAsync(request.EntryId, cancellationToken);

        if (entry is null)
        {
            return Result<AiEntryItem>.Failure("entry_not_found", "AI entry not found");
        }

        if (request.Action == AiEntryReviewAction.Approve)
        {
            if (string.IsNullOrWhiteSpace(entry.DialectText))
            {
                return Result<AiEntryItem>.Failure("text_required", "This entry has no text yet. Edit it first, which also approves it.");
            }

            entry.Approve(adminId.Value);
        }
        else
        {
            entry.Reject(adminId.Value);
        }

        await _entryRepository.SaveChangesAsync(cancellationToken);
        return Result<AiEntryItem>.Success(AiEntryMapping.ToItem(entry));
    }
}

public sealed record EditAiEntryCommand(Guid EntryId, string DialectText, string EnglishMeaning, string Arabizi)
    : IRequest<Result<AiEntryItem>>;

public sealed class EditAiEntryCommandValidator : AbstractValidator<EditAiEntryCommand>
{
    public EditAiEntryCommandValidator()
    {
        RuleFor(x => x.DialectText)
            .NotEmpty().WithMessage("Dialect text is required")
            .MaximumLength(200).WithMessage("Dialect text is too long");

        RuleFor(x => x.EnglishMeaning)
            .NotEmpty().WithMessage("English meaning is required")
            .MaximumLength(300).WithMessage("English meaning is too long");

        RuleFor(x => x.Arabizi)
            .NotEmpty().WithMessage("Chat-alphabet spelling is required")
            .MaximumLength(200).WithMessage("Chat-alphabet spelling is too long");
    }
}

public sealed class EditAiEntryCommandHandler : IRequestHandler<EditAiEntryCommand, Result<AiEntryItem>>
{
    private readonly IAiDictionaryRepository _entryRepository;
    private readonly ICurrentUserService _currentUserService;

    public EditAiEntryCommandHandler(IAiDictionaryRepository entryRepository, ICurrentUserService currentUserService)
    {
        _entryRepository = entryRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<AiEntryItem>> Handle(EditAiEntryCommand request, CancellationToken cancellationToken)
    {
        var adminId = _currentUserService.UserId;

        if (adminId is null)
        {
            return Result<AiEntryItem>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var entry = await _entryRepository.GetByIdAsync(request.EntryId, cancellationToken);

        if (entry is null)
        {
            return Result<AiEntryItem>.Failure("entry_not_found", "AI entry not found");
        }

        entry.EditAndApprove(request.DialectText, request.EnglishMeaning, request.Arabizi, adminId.Value);
        await _entryRepository.SaveChangesAsync(cancellationToken);

        return Result<AiEntryItem>.Success(AiEntryMapping.ToItem(entry));
    }
}
