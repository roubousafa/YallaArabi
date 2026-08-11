using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Features.Admin.AddLevel;

public sealed class AddLevelCommandHandler : IRequestHandler<AddLevelCommand, Result<AddLevelResponse>>
{
    private readonly ILevelRepository _levelRepository;

    public AddLevelCommandHandler(ILevelRepository levelRepository)
    {
        _levelRepository = levelRepository;
    }

    public async ValueTask<Result<AddLevelResponse>> Handle(AddLevelCommand request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim();

        var codeExists = await _levelRepository.CodeExistsAsync(code, cancellationToken);

        if (codeExists)
        {
            return Result<AddLevelResponse>.Failure("level_code_exists", $"A level with code '{code}' already exists");
        }

        var level = Level.Create(Guid.NewGuid(), code, request.Name.Trim(), request.OrderIndex);

        await _levelRepository.AddAsync(level, cancellationToken);
        await _levelRepository.SaveChangesAsync(cancellationToken);

        return Result<AddLevelResponse>.Success(new AddLevelResponse(level.Id, level.Code, level.Name));
    }
}