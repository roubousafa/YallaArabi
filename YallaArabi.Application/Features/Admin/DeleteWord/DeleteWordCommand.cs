using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteWord;

public sealed record DeleteWordCommand(Guid WordId) : IRequest<Result<Unit>>;