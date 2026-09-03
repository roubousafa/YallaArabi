using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.CreateStudent;

public sealed record CreateStudentCommand(
    string FullName,
    string Email,
    string Password,
    string? LevelCode
) : IRequest<Result<CreateStudentResponse>>;

public sealed record CreateStudentResponse(
    Guid UserId,
    string FullName,
    string Email,
    string? LevelCode
);