using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteStudent;

public sealed record DeleteStudentCommand(Guid UserId) : IRequest<Result<Unit>>;