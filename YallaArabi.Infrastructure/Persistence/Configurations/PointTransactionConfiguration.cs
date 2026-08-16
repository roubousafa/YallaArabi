using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class PointTransactionConfiguration : IEntityTypeConfiguration<PointTransaction>
{
    public void Configure(EntityTypeBuilder<PointTransaction> builder)
    {
        builder.ToTable("point_transactions");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.ActionKey)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.Note)
            .HasMaxLength(300);

        builder.HasIndex(p => new { p.UserId, p.EarnedAt });
        builder.HasIndex(p => new { p.UserId, p.ActionKey, p.RelatedEntityId });

        builder.Ignore(p => p.DomainEvents);
    }
}