using Mediator;
using YallaArabi.Application.Common;

namespace YallaArabi.Application.Features.Words.SaveWord;

public sealed record SaveWordCommand(Guid WordId) : IRequest<Result<SaveWordResponse>>;

public sealed record SaveWordResponse(Guid WordId, bool AlreadySaved);