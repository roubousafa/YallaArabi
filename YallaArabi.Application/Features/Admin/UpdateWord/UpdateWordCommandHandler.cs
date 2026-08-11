using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateWord;

public sealed class UpdateWordCommandHandler : IRequestHandler<UpdateWordCommand, Result<UpdateWordResponse>>
{
    private readonly IWordRepository _wordRepository;
    private readonly ILevelRepository _levelRepository;

    public UpdateWordCommandHandler(IWordRepository wordRepository, ILevelRepository levelRepository)
    {
        _wordRepository = wordRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<UpdateWordResponse>> Handle(UpdateWordCommand request, CancellationToken cancellationToken)
    {
        var word = await _wordRepository.GetByIdAsync(request.WordId, cancellationToken);

        if (word is null)
        {
            return Result<UpdateWordResponse>.Failure("word_not_found", "Word not found");
        }

        var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

        if (level is null)
        {
            return Result<UpdateWordResponse>.Failure("level_not_found", "Level not found");
        }

        word.Update(
            level.Id,
            request.ArabicText,
            request.Transliteration,
            request.EnglishMeaning,
            request.AudioUrl,
            request.ImageUrl);

        await _wordRepository.SaveChangesAsync(cancellationToken);

        return Result<UpdateWordResponse>.Success(new UpdateWordResponse(word.Id, word.ArabicText, level.Code));
    }
}