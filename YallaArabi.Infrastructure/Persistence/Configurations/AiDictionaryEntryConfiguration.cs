using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class AiDictionaryEntryConfiguration : IEntityTypeConfiguration<AiDictionaryEntry>
{
    public void Configure(EntityTypeBuilder<AiDictionaryEntry> builder)
    {
        builder.ToTable("ai_dictionary_entries");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Query).HasMaxLength(200).IsRequired();
        builder.Property(e => e.NormalizedQuery).HasMaxLength(200).IsRequired();
        builder.Property(e => e.Dialect).HasMaxLength(30).IsRequired();
        builder.Property(e => e.DialectText).HasMaxLength(200);
        builder.Property(e => e.EnglishMeaning).HasMaxLength(300);
        builder.Property(e => e.Arabizi).HasMaxLength(200);
        builder.Property(e => e.ImageUrl).HasMaxLength(500);
        builder.Property(e => e.ImageSource).HasMaxLength(20);
        builder.Property(e => e.ImageCredit).HasMaxLength(200);
        builder.Property(e => e.ImageCreditUrl).HasMaxLength(500);
        builder.Property(e => e.Status).HasMaxLength(20).IsRequired();
        builder.Property(e => e.Provider).HasMaxLength(30).IsRequired();

        builder.Ignore(e => e.IsVisibleToStudents);

        builder.HasIndex(e => new { e.NormalizedQuery, e.Dialect }).IsUnique();
        builder.HasIndex(e => new { e.RequestedByUserId, e.RequestedLocalDate });
        builder.HasIndex(e => new { e.Status, e.CreatedAt });
    }
}

public class AiModeSettingsConfiguration : IEntityTypeConfiguration<AiModeSettings>
{
    public void Configure(EntityTypeBuilder<AiModeSettings> builder)
    {
        builder.ToTable("ai_mode_settings");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Access).HasMaxLength(20).IsRequired();
    }
}
