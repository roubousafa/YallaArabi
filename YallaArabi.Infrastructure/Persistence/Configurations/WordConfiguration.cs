using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class WordConfiguration : IEntityTypeConfiguration<Word>
{
    public void Configure(EntityTypeBuilder<Word> builder)
    {
        builder.ToTable("words");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.ArabicText)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.Transliteration)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.EnglishMeaning)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(w => w.NormalizedArabic)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.NormalizedTransliteration)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.AudioUrl)
            .HasMaxLength(500);

        builder.Property(w => w.ImageUrl)
            .HasMaxLength(500);

        builder.Property(w => w.NormalizedEnglish)
          .HasMaxLength(300)
          .IsRequired();

        builder.HasIndex(w => w.NormalizedEnglish);

        builder.HasIndex(w => w.LevelId);
        builder.HasIndex(w => w.NormalizedArabic);
        builder.HasIndex(w => w.NormalizedTransliteration);
    }
}