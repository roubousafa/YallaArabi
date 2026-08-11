using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetWordsList;

public sealed class GetWordsListQueryHandler : IRequestHandler<GetWordsListQuery, Result<PagedWordsResponse>>
{
    private const int MaxPageSize = 100;

    private readonly IWordRepository _wordRepository;
    private readonly ILevelRepository _levelRepository;

    public GetWordsListQueryHandler(IWordRepository wordRepository, ILevelRepository levelRepository)
    {
        _wordRepository = wordRepository;
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<PagedWordsResponse>> Handle(GetWordsListQuery request, CancellationToken cancellationToken)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? 20 : Math.Min(request.PageSize, MaxPageSize);

        var allLevels = await _levelRepository.GetAllAsync(cancellationToken);
        var levelCodeById = allLevels.ToDictionary(l => l.Id, l => l.Code);

        Guid? levelId = null;

        if (!string.IsNullOrWhiteSpace(request.LevelCode))
        {
            var level = allLevels.FirstOrDefault(l => l.Code == request.LevelCode);

            if (level is null)
            {
                return Result<PagedWordsResponse>.Failure("level_not_found", "Level not found");
            }

            levelId = level.Id;
        }

        var (items, totalCount) = await _wordRepository.GetPagedAsync(
            request.SearchTerm, levelId, pageNumber, pageSize, cancellationToken);

        var results = items
            .Select(w => new AdminWordItem(
                w.Id,
                w.ArabicText,
                w.Transliteration,
                w.EnglishMeaning,
                levelCodeById.TryGetValue(w.LevelId, out var code) ? code : "?",
                w.AudioUrl,
                w.ImageUrl,
                w.CreatedAt))
            .ToList();

        var totalPages = totalCount == 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

        var response = new PagedWordsResponse(results, totalCount, pageNumber, pageSize, totalPages);
        return Result<PagedWordsResponse>.Success(response);
    }
}