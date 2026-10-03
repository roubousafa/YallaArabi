using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IUserRepository
{
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedTeamAsync(string? searchTerm, Guid? roleId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken ct = default);
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(string? searchTerm, Guid? levelId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task RemoveAsync(User user, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllByRoleAsync(string role, CancellationToken ct = default);
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetStudentsReportAsync(
    string? searchTerm,
    Guid? levelId,
    DateTime? fromDate,
    DateTime? toDate,
    int pageNumber,
    int pageSize,
    CancellationToken ct = default);

    Task<IReadOnlyList<User>> GetStudentsReportForExportAsync(
        string? searchTerm,
        Guid? levelId,
        DateTime? fromDate,
        DateTime? toDate,
        int maxRecords,
        CancellationToken ct = default);

    Task<User?> GetByFriendCodeAsync(string friendCode, CancellationToken ct = default);
    Task<bool> FriendCodeExistsAsync(string friendCode, CancellationToken ct = default);
}