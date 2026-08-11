using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteWord;

public sealed class DeleteWordCommandHandler : IRequestHandler<DeleteWordCommand, Result<Unit>>
{
    private readonly IWordRepository _wordRepository;

    public DeleteWordCommandHandler(IWordRepository wordRepository)
    {
        _wordRepository = wordRepository;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteWordCommand request, CancellationToken cancellationToken)
    {
        var word = await _wordRepository.GetByIdAsync(request.WordId, cancellationToken);

        if (word is null)
        {
            return Result<Unit>.Failure("word_not_found", "Word not found");
        }

        await _wordRepository.RemoveAsync(word, cancellationToken);
        await _wordRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}