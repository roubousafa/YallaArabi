using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class PointRuleConfiguration : IEntityTypeConfiguration<PointRule>
{
    public void Configure(EntityTypeBuilder<PointRule> builder)
    {
        builder.ToTable("point_rules");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.ActionKey)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.DisplayName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(300);

        builder.Property(p => p.Points)
            .IsRequired();

        builder.HasIndex(p => p.ActionKey).IsUnique();
    }
}