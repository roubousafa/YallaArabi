using Microsoft.EntityFrameworkCore;
using YallaArabi.Application.Interfaces;
using YallaArabi.Domain.Entities;
using YallaArabi.Domain.ValueObjects;
using YallaArabi.Infrastructure.Persistence;

namespace YallaArabi.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context) => _context = context;

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var emailVo = Email.Create(email);
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == emailVo, ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task<User?> GetByGoogleIdAsync(string googleId, CancellationToken ct = default) =>
        await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId, ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.AddAsync(user, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
    {
        var emailVo = Email.Create(email);
        return await _context.Users.AnyAsync(u => u.Email == emailVo, ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default) =>
        await _context.SaveChangesAsync(ct);

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm, string? role, Guid? levelId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(role))
        {
            query = query.Where(u => u.Role == role);
        }

        if (levelId.HasValue)
        {
            query = query.Where(u => u.CurrentLevelId == levelId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";
            query = query.Where(u => EF.Functions.Like(u.FullName, pattern));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task RemoveAsync(User user, CancellationToken ct = default)
    {
        _context.Users.Remove(user);
        await Task.CompletedTask;
    }

    public async Task<IReadOnlyList<User>> GetAllByRoleAsync(string role, CancellationToken ct = default) =>
        await _context.Users.Where(u => u.Role == role).ToListAsync(ct);
}