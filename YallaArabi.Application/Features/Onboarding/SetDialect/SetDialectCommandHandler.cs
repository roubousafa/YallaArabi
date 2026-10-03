using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Onboarding.SetDialect;

public sealed class SetDialectCommandHandler : IRequestHandler<SetDialectCommand, Result<SetDialectResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly ICurrentUserService _currentUserService;

    public SetDialectCommandHandler(
        IUserRepository userRepository,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<SetDialectResponse>> Handle(SetDialectCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<SetDialectResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var dialect = Dialects.Get(request.DialectKey);

        if (dialect is null)
        {
            return Result<SetDialectResponse>.Failure(
                "invalid_dialect",
                $"Invalid dialect. Valid options: {string.Join(", ", Dialects.All.Select(d => d.Key))}");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<SetDialectResponse>.Failure("user_not_found", "User not found");
        }

        var gender = VoiceGenders.Normalize(request.VoiceGender);

        user.SetPreferredDialect(dialect.Key);
        user.SetPreferredVoiceGender(gender);
        await _userRepository.SaveChangesAsync(cancellationToken);

        return Result<SetDialectResponse>.Success(
            new SetDialectResponse(dialect.Key, dialect.NameAr, dialect.NameEn, gender));
    }
}