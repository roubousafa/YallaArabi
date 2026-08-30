using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Admin.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid CategoryId) : IRequest<Result<Unit>>;