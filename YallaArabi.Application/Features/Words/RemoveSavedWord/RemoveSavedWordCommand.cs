using Mediator;

namespace YallaArabi.Application.Features.Words.RemoveSavedWord;

public sealed record RemoveSavedWordCommand(Guid WordId) : IRequest<Unit>;