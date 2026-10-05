using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class LessonQuestionConfiguration : IEntityTypeConfiguration<LessonQuestion>
{
    public void Configure(EntityTypeBuilder<LessonQuestion> builder)
    {
        builder.ToTable("lesson_questions");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.Type)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(q => q.QuestionText)
            .HasMaxLength(500);

        builder.Property(q => q.OptionA)
            .HasMaxLength(200);

        builder.Property(q => q.OptionB)
            .HasMaxLength(200);

        builder.Property(q => q.OptionC)
            .HasMaxLength(200);

        builder.Property(q => q.OptionD)
            .HasMaxLength(200);

        builder.Property(q => q.CorrectOption)
            .HasMaxLength(1);

        builder.Property(q => q.AudioUrl)
            .HasMaxLength(500);

        builder.Property(q => q.ImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(q => q.LessonId);
        builder.HasIndex(q => q.WordId);
    }
}
