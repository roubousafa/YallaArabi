namespace YallaArabi.Application.Interfaces;

public interface IAccountDeletionService
{
    Task DeleteUserDataAsync(Guid userId, CancellationToken ct = default);
}