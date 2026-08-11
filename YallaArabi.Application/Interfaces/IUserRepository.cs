using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken ct = default);
    Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(string? searchTerm, string? role, Guid? levelId, int pageNumber, int pageSize, CancellationToken ct = default);
    Task AddAsync(User user, CancellationToken ct = default);
    Task RemoveAsync(User user, CancellationToken ct = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<User>> GetAllByRoleAsync(string role, CancellationToken ct = default);
}