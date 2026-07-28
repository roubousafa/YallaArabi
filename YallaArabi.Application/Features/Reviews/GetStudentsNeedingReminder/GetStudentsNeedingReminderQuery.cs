using Mediator;

namespace YallaArabi.Application.Features.Reviews.GetStudentsNeedingReminder;

public sealed record GetStudentsNeedingReminderQuery : IRequest<IReadOnlyList<Guid>>;