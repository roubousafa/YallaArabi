using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Words.SearchWords;

namespace YallaArabi.Application.Features.Words.GetWordById;

public sealed record GetWordByIdQuery(Guid WordId) : IRequest<Result<WordSearchResult>>;