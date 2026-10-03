using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class WordAudioConfiguration : IEntityTypeConfiguration<WordAudio>
{
    public void Configure(EntityTypeBuilder<WordAudio> builder)
    {
        builder.ToTable("word_audios");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Dialect).HasMaxLength(30).IsRequired();
        builder.Property(a => a.Gender).HasMaxLength(10).IsRequired();
        builder.Property(a => a.Source).HasMaxLength(10).IsRequired();
        builder.Property(a => a.AudioUrl).HasMaxLength(500).IsRequired();
        builder.Property(a => a.Status).HasMaxLength(20).IsRequired();
        builder.Property(a => a.ReviewNote).HasMaxLength(300);

        builder.HasIndex(a => new { a.WordId, a.Dialect, a.Gender });
        builder.HasIndex(a => a.Status);

        builder.Ignore(a => a.DomainEvents);
    }
}