using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Onboarding;

public sealed class SelectLevelCommandHandler : IRequestHandler<SelectLevelCommand, Result<SelectLevelResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILevelRepository _levelRepository;
    private readonly ICurrentUserService _currentUserService;

    public SelectLevelCommandHandler(
        IUserRepository userRepository,
        ILevelRepository levelRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _levelRepository = levelRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SelectLevelResponse>> Handle(SelectLevelCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<SelectLevelResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<SelectLevelResponse>.Failure("user_not_found", "User not found");
        }

        var selectedOption = SelfAssessmentOptions.All.First(o => o.Key == request.AssessmentKey);
        var level = await _levelRepository.GetByCodeAsync(selectedOption.LevelCode, cancellationToken);

        if (level is null)
        {
            return Result<SelectLevelResponse>.Failure("level_not_found", "Could not determine a matching level");
        }

        user.SetCurrentLevel(level.Id);
        await _userRepository.SaveChangesAsync(cancellationToken);

        var response = new SelectLevelResponse(level.Code, level.Name);
        return Result<SelectLevelResponse>.Success(response);
    }
}