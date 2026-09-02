using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class Role : AggregateRoot
{
    private readonly List<RolePermission> _permissions = new();

    public string NameEn { get; private set; } = default!;
    public string NameAr { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsSystemRole { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    private Role() { }

    public static Role Create(string nameEn, string nameAr, string? description, bool isSystemRole = false)
    {
        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("English name is required", nameof(nameEn));

        if (string.IsNullOrWhiteSpace(nameAr))
            throw new ArgumentException("Arabic name is required", nameof(nameAr));

        return new Role
        {
            Id = Guid.NewGuid(),
            NameEn = nameEn.Trim(),
            NameAr = nameAr.Trim(),
            Description = description?.Trim(),
            IsSystemRole = isSystemRole,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void Update(string nameEn, string nameAr, string? description)
    {
        if (IsSystemRole)
            throw new InvalidOperationException("System roles cannot be modified");

        if (string.IsNullOrWhiteSpace(nameEn))
            throw new ArgumentException("English name is required", nameof(nameEn));

        if (string.IsNullOrWhiteSpace(nameAr))
            throw new ArgumentException("Arabic name is required", nameof(nameAr));

        NameEn = nameEn.Trim();
        NameAr = nameAr.Trim();
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPermissions(IEnumerable<string> permissionKeys)
    {
        if (IsSystemRole)
            throw new InvalidOperationException("System role permissions cannot be modified");

        var desired = permissionKeys
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Select(k => k.Trim())
            .Distinct()
            .ToHashSet();

        _permissions.RemoveAll(p => !desired.Contains(p.PermissionKey));

        var existing = _permissions.Select(p => p.PermissionKey).ToHashSet();

        foreach (var key in desired)
        {
            if (existing.Contains(key))
            {
                continue;
            }

            _permissions.Add(RolePermission.Create(Id, key));
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasPermission(string permissionKey) =>
        IsSystemRole || _permissions.Any(p => p.PermissionKey == permissionKey);
}