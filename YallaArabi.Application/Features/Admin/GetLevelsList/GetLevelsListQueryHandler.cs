using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.GetLevelsList;

public sealed class GetLevelsListQueryHandler : IRequestHandler<GetLevelsListQuery, Result<IReadOnlyList<AdminLevelItem>>>
{
    private readonly ILevelRepository _levelRepository;

    public GetLevelsListQueryHandler(ILevelRepository levelRepository)
    {
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<IReadOnlyList<AdminLevelItem>>> Handle(GetLevelsListQuery request, CancellationToken cancellationToken)
    {
        var levels = await _levelRepository.GetAllAsync(cancellationToken);
        var results = new List<AdminLevelItem>();

        foreach (var level in levels)
        {
            var counts = await _levelRepository.GetContentCountsAsync(level.Id, cancellationToken);

            var canBeDeleted = counts.CategoriesCount == 0
                && counts.LessonsCount == 0
                && counts.WordsCount == 0
                && counts.StudentsCount == 0;

            results.Add(new AdminLevelItem(
                level.Id,
                level.Code,
                level.Name,
                level.OrderIndex,
                counts.CategoriesCount,
                counts.LessonsCount,
                counts.WordsCount,
                counts.StudentsCount,
                canBeDeleted));
        }

        return Result<IReadOnlyList<AdminLevelItem>>.Success(results);
    }
}