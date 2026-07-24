using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class StudentLessonProgressConfiguration : IEntityTypeConfiguration<StudentLessonProgress>
{
    public void Configure(EntityTypeBuilder<StudentLessonProgress> builder)
    {
        builder.ToTable("student_lesson_progress");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.CompletedAt)
            .IsRequired();

        builder.HasIndex(p => new { p.UserId, p.LessonId }).IsUnique();

        builder.Ignore(p => p.DomainEvents);
    }
}