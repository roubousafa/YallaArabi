using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class LessonWordConfiguration : IEntityTypeConfiguration<LessonWord>
{
    public void Configure(EntityTypeBuilder<LessonWord> builder)
    {
        builder.ToTable("lesson_words");
        builder.HasKey(w => w.Id);

        builder.HasIndex(w => new { w.LessonId, w.WordId }).IsUnique();
        builder.HasIndex(w => w.WordId);
    }
}
