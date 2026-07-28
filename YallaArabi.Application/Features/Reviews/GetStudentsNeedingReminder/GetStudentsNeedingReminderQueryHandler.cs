using Mediator;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Reviews.GetStudentsNeedingReminder;

public sealed class GetStudentsNeedingReminderQueryHandler : IRequestHandler<GetStudentsNeedingReminderQuery, IReadOnlyList<Guid>>
{
    private const int OverdueThresholdDays = 7;

    private readonly IWordReviewRepository _wordReviewRepository;

    public GetStudentsNeedingReminderQueryHandler(IWordReviewRepository wordReviewRepository)
    {
        _wordReviewRepository = wordReviewRepository;
    }

    public async ValueTask<IReadOnlyList<Guid>> Handle(GetStudentsNeedingReminderQuery request, CancellationToken cancellationToken)
    {
        var overdueBefore = DateTime.UtcNow.AddDays(-OverdueThresholdDays);

        return await _wordReviewRepository.GetUserIdsWithOverdueReviewsAsync(overdueBefore, cancellationToken);
    }
}