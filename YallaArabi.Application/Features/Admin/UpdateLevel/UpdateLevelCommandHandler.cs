using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Admin.UpdateLevel;

public sealed class UpdateLevelCommandHandler : IRequestHandler<UpdateLevelCommand, Result<UpdateLevelResponse>>
{
    private readonly ILevelRepository _levelRepository;

    public UpdateLevelCommandHandler(ILevelRepository levelRepository)
    {
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<UpdateLevelResponse>> Handle(UpdateLevelCommand request, CancellationToken cancellationToken)
    {
        var level = await _levelRepository.GetByIdAsync(request.LevelId, cancellationToken);

        if (level is null)
        {
            return Result<UpdateLevelResponse>.Failure("level_not_found", "Level not found");
        }

        level.Update(request.Name, request.OrderIndex);
        await _levelRepository.SaveChangesAsync(cancellationToken);

        return Result<UpdateLevelResponse>.Success(
            new UpdateLevelResponse(level.Id, level.Code, level.Name, level.OrderIndex));
    }
}