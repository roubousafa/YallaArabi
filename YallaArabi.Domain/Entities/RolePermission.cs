using YallaArabi.Domain.Common;

namespace YallaArabi.Domain.Entities;

public sealed class RolePermission : Entity
{
    public Guid RoleId { get; private set; }
    public string PermissionKey { get; private set; } = default!;

    private RolePermission() { }

    public static RolePermission Create(Guid roleId, string permissionKey)
    {
        if (string.IsNullOrWhiteSpace(permissionKey))
            throw new ArgumentException("Permission key is required", nameof(permissionKey));

        return new RolePermission
        {
            Id = Guid.NewGuid(),
            RoleId = roleId,
            PermissionKey = permissionKey.Trim()
        };
    }
}