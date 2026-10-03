namespace YallaArabi.Application.Interfaces;

public interface IFriendCodeService
{
    Task<string> GenerateUniqueAsync(CancellationToken ct = default);
}