using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.DeleteLevel;

public sealed class DeleteLevelCommandHandler : IRequestHandler<DeleteLevelCommand, Result<Unit>>
{
    private readonly ILevelRepository _levelRepository;

    public DeleteLevelCommandHandler(ILevelRepository levelRepository)
    {
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<Unit>> Handle(DeleteLevelCommand request, CancellationToken cancellationToken)
    {
        var level = await _levelRepository.GetByIdAsync(request.LevelId, cancellationToken);

        if (level is null)
        {
            return Result<Unit>.Failure("level_not_found", "Level not found");
        }

        var counts = await _levelRepository.GetContentCountsAsync(level.Id, cancellationToken);

        var hasContent = counts.CategoriesCount > 0
            || counts.LessonsCount > 0
            || counts.WordsCount > 0
            || counts.StudentsCount > 0;

        if (hasContent)
        {
            var details = $"categories: {counts.CategoriesCount}, lessons: {counts.LessonsCount}, words: {counts.WordsCount}, students: {counts.StudentsCount}";

            return Result<Unit>.Failure(
                "level_has_content",
                $"Cannot delete this level because it still has related content ({details}). Please move or delete the related content first.");
        }

        await _levelRepository.RemoveAsync(level, cancellationToken);
        await _levelRepository.SaveChangesAsync(cancellationToken);

        return Result<Unit>.Success(Unit.Value);
    }
}