using Mediator;
using YallaArabi.Application.Common;
using YallaArabi.Application.Interfaces;

namespace YallaArabi.Application.Features.Profile.SetAvatar;

// Content is null to remove the current avatar.
public sealed record SetAvatarCommand(Stream? Content, string? FileName, string? ContentType) : IRequest<Result<AvatarResponse>>;

public sealed record AvatarResponse(string? AvatarUrl);

public sealed class SetAvatarCommandHandler : IRequestHandler<SetAvatarCommand, Result<AvatarResponse>>
{
    private const string AvatarFolder = "avatars";

    private readonly IUserRepository _userRepository;
    private readonly IFileStorage _fileStorage;
    private readonly ICurrentUserService _currentUserService;

    public SetAvatarCommandHandler(IUserRepository userRepository, IFileStorage fileStorage, ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _fileStorage = fileStorage;
        _currentUserService = currentUserService;
    }

    public async ValueTask<Result<AvatarResponse>> Handle(SetAvatarCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId is null)
        {
            return Result<AvatarResponse>.Failure("unauthorized", "You must be logged in to perform this action");
        }

        var user = await _userRepository.GetByIdAsync(currentUserId.Value, cancellationToken);

        if (user is null)
        {
            return Result<AvatarResponse>.Failure("user_not_found", "User not found");
        }

        var previousUrl = user.AvatarUrl;
        string? newUrl = null;

        if (request.Content is not null)
        {
            var upload = await _fileStorage.UploadAsync(
                request.Content,
                request.FileName ?? "avatar",
                request.ContentType ?? "application/octet-stream",
                AvatarFolder,
                cancellationToken);

            newUrl = upload.Url;
        }

        user.SetAvatarUrl(newUrl);
        await _userRepository.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(previousUrl))
        {
            await _fileStorage.DeleteAsync(previousUrl, cancellationToken);
        }

        return Result<AvatarResponse>.Success(new AvatarResponse(user.AvatarUrl));
    }
}
