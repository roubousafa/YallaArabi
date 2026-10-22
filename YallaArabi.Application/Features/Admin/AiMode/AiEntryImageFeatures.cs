using FluentValidation;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Admin.AiMode;

internal static class AiEntryImageCleanup
{
    // Only photos we uploaded live on our storage; Pexels photos are links and have nothing to delete.
    public static async Task DeleteIfOwnedAsync(IFileStorage fileStorage, string? url, string? source, CancellationToken ct)
    {
        if (source == AiImageSources.Admin && !string.IsNullOrWhiteSpace(url))
        {
            await fileStorage.DeleteAsync(url, ct);
        }
    }
}

public sealed record UploadAiEntryImageCommand(Guid EntryId, Stream Content, string FileName, string ContentType)
    : IRequest<Result<AiEntryItem>>;

public sealed class UploadAiEntryImageCommandHandler : IRequestHandler<UploadAiEntryImageCommand, Result<AiEntryItem>>
{
    private const string ImageFolder = "ai-images";

    private readonly IAiDictionaryRepository _entryRepository;
    private readonly IFileStorage _fileStorage;

    public UploadAiEntryImageCommandHandler(IAiDictionaryRepository entryRepository, IFileStorage fileStorage)
    {
        _entryRepository = entryRepository;
        _fileStorage = fileStorage;
    }

    public async ValueTask<Result<AiEntryItem>> Handle(UploadAiEntryImageCommand request, CancellationToken cancellationToken)
    {
        var entry = await _entryRepository.GetByIdAsync(request.EntryId, cancellationToken);

        if (entry is null)
        {
            return Result<AiEntryItem>.Failure("entry_not_found", "AI entry not found");
        }

        var (previousUrl, previousSource) = (entry.ImageUrl, entry.ImageSource);

        var upload = await _fileStorage.UploadAsync(
            request.Content, request.FileName, request.ContentType, ImageFolder, cancellationToken);

        entry.SetImage(upload.Url, AiImageSources.Admin, null, null);
        await _entryRepository.SaveChangesAsync(cancellationToken);
        await AiEntryImageCleanup.DeleteIfOwnedAsync(_fileStorage, previousUrl, previousSource, cancellationToken);

        return Result<AiEntryItem>.Success(AiEntryMapping.ToItem(entry));
    }
}

public sealed record RemoveAiEntryImageCommand(Guid EntryId) : IRequest<Result<AiEntryItem>>;

public sealed class RemoveAiEntryImageCommandHandler : IRequestHandler<RemoveAiEntryImageCommand, Result<AiEntryItem>>
{
    private readonly IAiDictionaryRepository _entryRepository;
    private readonly IFileStorage _fileStorage;

    public RemoveAiEntryImageCommandHandler(IAiDictionaryRepository entryRepository, IFileStorage fileStorage)
    {
        _entryRepository = entryRepository;
        _fileStorage = fileStorage;
    }

    public async ValueTask<Result<AiEntryItem>> Handle(RemoveAiEntryImageCommand request, CancellationToken cancellationToken)
    {
        var entry = await _entryRepository.GetByIdAsync(request.EntryId, cancellationToken);

        if (entry is null)
        {
            return Result<AiEntryItem>.Failure("entry_not_found", "AI entry not found");
        }

        var (previousUrl, previousSource) = (entry.ImageUrl, entry.ImageSource);

        entry.RemoveImage();
        await _entryRepository.SaveChangesAsync(cancellationToken);
        await AiEntryImageCleanup.DeleteIfOwnedAsync(_fileStorage, previousUrl, previousSource, cancellationToken);

        return Result<AiEntryItem>.Success(AiEntryMapping.ToItem(entry));
    }
}

// Query is optional; without it the photo search uses the entry's English meaning.
public sealed record FindAiEntryImageCommand(Guid EntryId, string? Query) : IRequest<Result<AiEntryItem>>;

public sealed class FindAiEntryImageCommandValidator : AbstractValidator<FindAiEntryImageCommand>
{
    public FindAiEntryImageCommandValidator()
    {
        RuleFor(x => x.Query).MaximumLength(100).WithMessage("Image search is too long");
    }
}

public sealed class FindAiEntryImageCommandHandler : IRequestHandler<FindAiEntryImageCommand, Result<AiEntryItem>>
{
    private readonly IAiDictionaryRepository _entryRepository;
    private readonly IImageSearchProvider _imageProvider;
    private readonly IFileStorage _fileStorage;

    public FindAiEntryImageCommandHandler(
        IAiDictionaryRepository entryRepository,
        IImageSearchProvider imageProvider,
        IFileStorage fileStorage)
    {
        _entryRepository = entryRepository;
        _imageProvider = imageProvider;
        _fileStorage = fileStorage;
    }

    public async ValueTask<Result<AiEntryItem>> Handle(FindAiEntryImageCommand request, CancellationToken cancellationToken)
    {
        if (!_imageProvider.IsEnabled)
        {
            return Result<AiEntryItem>.Failure("image_search_disabled", "Automatic photos are not configured on the server.");
        }

        var entry = await _entryRepository.GetByIdAsync(request.EntryId, cancellationToken);

        if (entry is null)
        {
            return Result<AiEntryItem>.Failure("entry_not_found", "AI entry not found");
        }

        var query = string.IsNullOrWhiteSpace(request.Query) ? entry.EnglishMeaning : request.Query;

        if (string.IsNullOrWhiteSpace(query))
        {
            return Result<AiEntryItem>.Failure("image_query_required", "This entry has no English meaning; send a search word.");
        }

        ImageSearchResult? image;

        try
        {
            image = await _imageProvider.FindAsync(query, cancellationToken);
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            return Result<AiEntryItem>.Failure("image_search_unavailable", "The photo service is not responding. Try again later.");
        }

        if (image is null)
        {
            return Result<AiEntryItem>.Failure("image_not_found", "No photo matched this search. Try other words.");
        }

        var (previousUrl, previousSource) = (entry.ImageUrl, entry.ImageSource);

        entry.SetImage(image.Url, image.Source, image.Credit, image.CreditUrl);
        await _entryRepository.SaveChangesAsync(cancellationToken);
        await AiEntryImageCleanup.DeleteIfOwnedAsync(_fileStorage, previousUrl, previousSource, cancellationToken);

        return Result<AiEntryItem>.Success(AiEntryMapping.ToItem(entry));
    }
}
