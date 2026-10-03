using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class VoiceProfileConfiguration : IEntityTypeConfiguration<VoiceProfile>
{
    public void Configure(EntityTypeBuilder<VoiceProfile> builder)
    {
        builder.ToTable("voice_profiles");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.DisplayName).HasMaxLength(150).IsRequired();
        builder.Property(v => v.Dialect).HasMaxLength(30).IsRequired();
        builder.Property(v => v.Gender).HasMaxLength(10).IsRequired();
        builder.Property(v => v.Source).HasMaxLength(10).IsRequired();
        builder.Property(v => v.Notes).HasMaxLength(500);

        builder.HasIndex(v => new { v.Dialect, v.Gender });

        builder.Ignore(v => v.DomainEvents);
    }
}