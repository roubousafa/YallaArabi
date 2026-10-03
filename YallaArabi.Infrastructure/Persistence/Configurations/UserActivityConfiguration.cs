using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
{
    public void Configure(EntityTypeBuilder<UserActivity> builder)
    {
        builder.ToTable("user_activities");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.ActivityType)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(a => new { a.UserId, a.LocalDate });
        builder.HasIndex(a => new { a.UserId, a.ActivityType, a.RelatedEntityId, a.LocalDate });

        builder.Ignore(a => a.DomainEvents);
    }
}
