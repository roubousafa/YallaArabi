using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.UpdateStudent;

public sealed record UpdateStudentCommand(
    Guid UserId,
    string FullName,
    string? LevelCode
) : IRequest<Result<Unit>>;