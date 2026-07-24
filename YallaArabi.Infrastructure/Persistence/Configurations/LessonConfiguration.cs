using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("lessons");
        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(l => l.OrderIndex)
            .IsRequired();

        builder.Property(l => l.PointsReward)
            .IsRequired();

        builder.HasIndex(l => l.CategoryId);
    }
}