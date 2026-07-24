using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Onboarding;

public sealed class SetLearningMotivationCommandHandler : IRequestHandler<SetLearningMotivationCommand, Result<SetLearningMotivationResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public SetLearningMotivationCommandHandler(IUserRepository userRepository, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SetLearningMotivationResponse>> Handle(SetLearningMotivationCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<SetLearningMotivationResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<SetLearningMotivationResponse>.Failure("user_not_found", "User not found");
        }

        var selectedOption = LearningMotivationOptions.All.First(o => o.Key == request.MotivationKey);

        user.SetLearningMotivation(selectedOption.Key);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<SetLearningMotivationResponse>.Success(new SetLearningMotivationResponse(selectedOption.Key, selectedOption.Title));
    }
}