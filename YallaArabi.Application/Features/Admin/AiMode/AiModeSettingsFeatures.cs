using FluentValidation;
using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Features.Admin.AiMode;

public sealed record AiModeSettingsResponse(
    string Access,
    int DailyLimit,
    string Provider,
    string ImageProvider,
    IReadOnlyList<string> AccessOptions,
    DateTime UpdatedAt
);

public sealed record GetAiModeSettingsQuery : IRequest<Result<AiModeSettingsResponse>>;

public sealed class GetAiModeSettingsQueryHandler : IRequestHandler<GetAiModeSettingsQuery, Result<AiModeSettingsResponse>>
{
    private readonly IAiModeSettingsRepository _settingsRepository;
    private readonly IDialectAiProvider _aiProvider;
    private readonly IImageSearchProvider _imageProvider;

    public GetAiModeSettingsQueryHandler(IAiModeSettingsRepository settingsRepository, IDialectAiProvider aiProvider, IImageSearchProvider imageProvider)
    {
        _settingsRepository = settingsRepository;
        _aiProvider = aiProvider;
        _imageProvider = imageProvider;
    }

    public async ValueTask<Result<AiModeSettingsResponse>> Handle(GetAiModeSettingsQuery request, CancellationToken cancellationToken)
    {
        var settings = await _settingsRepository.GetAsync(cancellationToken);

        return Result<AiModeSettingsResponse>.Success(new AiModeSettingsResponse(
            settings.Access, settings.DailyLimit, _aiProvider.Name, _imageProvider.Name, AiModeAccess.All, settings.UpdatedAt));
    }
}

public sealed record UpdateAiModeSettingsCommand(string Access, int DailyLimit) : IRequest<Result<AiModeSettingsResponse>>;

public sealed class UpdateAiModeSettingsCommandValidator : AbstractValidator<UpdateAiModeSettingsCommand>
{
    public UpdateAiModeSettingsCommandValidator()
    {
        RuleFor(x => x.Access)
            .Must(AiModeAccess.IsSupported)
            .WithMessage($"Access must be one of: {string.Join(", ", AiModeAccess.All)}");

        RuleFor(x => x.DailyLimit)
            .InclusiveBetween(0, 1000).WithMessage("Daily limit must be between 0 (no limit) and 1000");
    }
}

public sealed class UpdateAiModeSettingsCommandHandler : IRequestHandler<UpdateAiModeSettingsCommand, Result<AiModeSettingsResponse>>
{
    private readonly IAiModeSettingsRepository _settingsRepository;
    private readonly IDialectAiProvider _aiProvider;
    private readonly IImageSearchProvider _imageProvider;
    private readonly ICurrentUserService _currentUserService;

    public UpdateAiModeSettingsCommandHandler(
        IAiModeSettingsRepository settingsRepository,
        IDialectAiProvider aiProvider,
        IImageSearchProvider imageProvider,
        ICurrentUserService currentUserService)
    {
        _settingsRepository = settingsRepository;
        _aiProvider = aiProvider;
        _imageProvider = imageProvider;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<AiModeSettingsResponse>> Handle(UpdateAiModeSettingsCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<AiModeSettingsResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var settings = await _settingsRepository.GetAsync(cancellationToken);
        settings.Update(request.Access, request.DailyLimit, currentUserId.Value);
        await _settingsRepository.SaveChangesAsync(cancellationToken);

        return Result<AiModeSettingsResponse>.Success(new AiModeSettingsResponse(
            settings.Access, settings.DailyLimit, _aiProvider.Name, _imageProvider.Name, AiModeAccess.All, settings.UpdatedAt));
    }
}
