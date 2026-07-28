using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Words.SaveWord;

public sealed class SaveWordCommandHandler : IRequestHandler<SaveWordCommand, Result<SaveWordResponse>>
{
    private readonly IWordRepository _wordRepository;
    private readonly ISavedWordRepository _savedWordRepository;
    private readonly ICurrentUserService _currentUserService;

    public SaveWordCommandHandler(
        IWordRepository wordRepository,
        ISavedWordRepository savedWordRepository,
        ICurrentUserService currentUserService)
    {
        _wordRepository = wordRepository;
        _savedWordRepository = savedWordRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SaveWordResponse>> Handle(SaveWordCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<SaveWordResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var word = await _wordRepository.GetByIdAsync(request.WordId, cancellationToken);

        if (word is null)
        {
            return Result<SaveWordResponse>.Failure("word_not_found", "Word not found");
        }

        var alreadySaved = await _savedWordRepository.ExistsAsync(currentUserId.Value, request.WordId, cancellationToken);

        if (alreadySaved)
        {
            return Result<SaveWordResponse>.Success(new SaveWordResponse(request.WordId, true));
        }

        var savedWord = SavedWord.Create(currentUserId.Value, request.WordId);
        await _savedWordRepository.AddAsync(savedWord, cancellationToken);
        await _savedWordRepository.SaveChangesAsync(cancellationToken);

        return Result<SaveWordResponse>.Success(new SaveWordResponse(request.WordId, false));
    }
}