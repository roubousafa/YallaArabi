using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.AddWord;

public sealed class AddWordCommandHandler : IRequestHandler<AddWordCommand, Result<AddWordResponse>>
{
    private readonly ILevelRepository _levelRepository;
    private readonly IWordRepository _wordRepository;

    public AddWordCommandHandler(ILevelRepository levelRepository, IWordRepository wordRepository)
    {
        _levelRepository = levelRepository;
        _wordRepository = wordRepository;
    }

    public async ValueTask<Result<AddWordResponse>> Handle(AddWordCommand request, CancellationToken cancellationToken)
    {
        var level = await _levelRepository.GetByCodeAsync(request.LevelCode, cancellationToken);

        if (level is null)
        {
            return Result<AddWordResponse>.Failure("level_not_found", "Level not found");
        }

        var word = Word.Create(
            level.Id,
            request.ArabicText,
            request.Transliteration,
            request.EnglishMeaning,
            request.AudioUrl,
            request.ImageUrl);

        await _wordRepository.AddAsync(word, cancellationToken);
        await _wordRepository.SaveChangesAsync(cancellationToken);

        return Result<AddWordResponse>.Success(new AddWordResponse(word.Id, word.ArabicText, level.Code));
    }
}