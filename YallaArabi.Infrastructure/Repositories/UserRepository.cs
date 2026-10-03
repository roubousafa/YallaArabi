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

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetPagedTeamAsync(
    string? searchTerm, Guid? roleId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Users.Where(u => u.Role == "Admin");

        if (roleId.HasValue)
        {
            query = query.Where(u => u.RoleId == roleId.Value);
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
     string? searchTerm, Guid? levelId, int pageNumber, int pageSize, CancellationToken ct = default)
    {
        var query = _context.Users.Where(u => u.Role == "Student");

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

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> GetStudentsReportAsync(
    string? searchTerm,
    Guid? levelId,
    DateTime? fromDate,
    DateTime? toDate,
    int pageNumber,
    int pageSize,
    CancellationToken ct = default)
    {
        var query = BuildStudentsReportQuery(searchTerm, levelId, fromDate, toDate);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<IReadOnlyList<User>> GetStudentsReportForExportAsync(
        string? searchTerm,
        Guid? levelId,
        DateTime? fromDate,
        DateTime? toDate,
        int maxRecords,
        CancellationToken ct = default)
    {
        return await BuildStudentsReportQuery(searchTerm, levelId, fromDate, toDate)
            .OrderByDescending(u => u.CreatedAt)
            .Take(maxRecords)
            .ToListAsync(ct);
    }

    private IQueryable<User> BuildStudentsReportQuery(
        string? searchTerm, Guid? levelId, DateTime? fromDate, DateTime? toDate)
    {
        var query = _context.Users.Where(u => u.Role == "Student");

        if (levelId.HasValue)
        {
            query = query.Where(u => u.CurrentLevelId == levelId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(u => u.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(u => u.CreatedAt <= toDate.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.Trim()}%";
            query = query.Where(u => EF.Functions.Like(u.FullName, pattern));
        }

        return query;
    }


    public async Task<IReadOnlyList<User>> GetAllByRoleAsync(string role, CancellationToken ct = default) =>
        await _context.Users.Where(u => u.Role == role).ToListAsync(ct);

    public async Task<User?> GetByFriendCodeAsync(string friendCode, CancellationToken ct = default) =>
    await _context.Users.FirstOrDefaultAsync(u => u.FriendCode == friendCode, ct);

    public async Task<bool> FriendCodeExistsAsync(string friendCode, CancellationToken ct = default) =>
        await _context.Users.AnyAsync(u => u.FriendCode == friendCode, ct);

}