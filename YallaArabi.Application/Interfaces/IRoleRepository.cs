using YallaArabi.Domain.Entities;

namespace YallaArabi.Application.Interfaces;

public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Role?> GetByNameEnAsync(string nameEn, CancellationToken ct = default);
    Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken ct = default);
    Task<bool> NameExistsAsync(string nameEn, Guid? excludeRoleId = null, CancellationToken ct = default);
    Task<int> CountUsersInRoleAsync(Guid roleId, CancellationToken ct = default);
    Task<IReadOnlyList<string>> GetPermissionKeysAsync(Guid roleId, CancellationToken ct = default);
    Task AddAsync(Role role, CancellationToken ct = default);
    Task RemoveAsync(Role role, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}