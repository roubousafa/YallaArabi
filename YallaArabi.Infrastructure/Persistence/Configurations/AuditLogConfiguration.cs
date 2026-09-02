using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.UserFullName).HasMaxLength(150).IsRequired();
        builder.Property(a => a.UserEmail).HasMaxLength(200).IsRequired();
        builder.Property(a => a.Action).HasMaxLength(200).IsRequired();
        builder.Property(a => a.EntityType).HasMaxLength(50);
        builder.Property(a => a.EntityId).HasMaxLength(100);
        builder.Property(a => a.HttpMethod).HasMaxLength(10).IsRequired();
        builder.Property(a => a.Path).HasMaxLength(500).IsRequired();
        builder.Property(a => a.IpAddress).HasMaxLength(50);

        builder.HasIndex(a => a.OccurredAt);
        builder.HasIndex(a => new { a.UserId, a.OccurredAt });

        builder.Ignore(a => a.DomainEvents);
    }
}