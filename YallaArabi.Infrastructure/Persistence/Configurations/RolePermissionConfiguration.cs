using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.PermissionKey).HasMaxLength(100).IsRequired();

        builder.HasIndex(p => new { p.RoleId, p.PermissionKey }).IsUnique();
    }
}