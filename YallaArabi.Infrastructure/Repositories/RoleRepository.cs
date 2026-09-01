using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;

    public RoleRepository(AppDbContext context) => _context = context;

    public async Task<Role?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<Role?> GetByNameEnAsync(string nameEn, CancellationToken ct = default) =>
        await _context.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.NameEn == nameEn, ct);

    public async Task<IReadOnlyList<Role>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Roles
            .Include(r => r.Permissions)
            .OrderByDescending(r => r.IsSystemRole)
            .ThenBy(r => r.NameEn)
            .ToListAsync(ct);

    public async Task<bool> NameExistsAsync(string nameEn, Guid? excludeRoleId = null, CancellationToken ct = default) =>
        await _context.Roles
            .AnyAsync(r => r.NameEn == nameEn && (excludeRoleId == null || r.Id != excludeRoleId.Value), ct);

    public async Task<int> CountUsersInRoleAsync(Guid roleId, CancellationToken ct = default) =>
        await _context.Users.CountAsync(u => u.RoleId == roleId, ct);

    public async Task<IReadOnlyList<string>> GetPermissionKeysAsync(Guid roleId, CancellationToken ct = default) =>
        await _context.RolePermissions
            .Where(p => p.RoleId == roleId)
            .Select(p => p.PermissionKey)
            .ToListAsync(ct);

    public async Task AddAsync(Role role, CancellationToken ct = default) =>
        await _context.Roles.AddAsync(role, ct);

    public async Task RemoveAsync(Role role, CancellationToken ct = default)
    {
        _context.Roles.Remove(role);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);
}