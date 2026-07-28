using Mediator;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Words.RemoveSavedWord;

public sealed class RemoveSavedWordCommandHandler : IRequestHandler<RemoveSavedWordCommand, Unit>
{
    private readonly ISavedWordRepository _savedWordRepository;
    private readonly ICurrentUserService _currentUserService;

    public RemoveSavedWordCommandHandler(
        ISavedWordRepository savedWordRepository,
        ICurrentUserService currentUserService)
    {
        _savedWordRepository = savedWordRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Unit> Handle(RemoveSavedWordCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Unit.Value;
        }

        var savedWord = await _savedWordRepository.GetAsync(currentUserId.Value, request.WordId, cancellationToken);

        if (savedWord is not null)
        {
            await _savedWordRepository.RemoveAsync(savedWord, cancellationToken);
            await _savedWordRepository.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}