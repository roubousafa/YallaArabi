using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Features.Words.SearchWords;

namespace YallaArabi.Application.Features.Words.GetSavedWords;

public sealed record GetSavedWordsQuery : IRequest<Result<IReadOnlyList<WordSearchResult>>>;