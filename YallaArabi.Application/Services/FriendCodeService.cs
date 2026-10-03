using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Common;

namespace YallaArabi.Application.Services;

public sealed class FriendCodeService : IFriendCodeService
{
    private const int MaxAttempts = 10;

    private readonly IUserRepository _userRepository;

    public FriendCodeService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string> GenerateUniqueAsync(CancellationToken ct = default)
    {
        for (var attempt = 0; attempt < MaxAttempts; attempt++)
        {
            var code = FriendCodes.Generate();
            var exists = await _userRepository.FriendCodeExistsAsync(code, ct);

            if (!exists)
            {
                return code;
            }
        }

        return FriendCodes.Generate() + RandomSuffix();
    }

    private static string RandomSuffix() =>
        Guid.NewGuid().ToString("N")[..3].ToUpperInvariant();
}