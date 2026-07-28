using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class WordReviewConfiguration : IEntityTypeConfiguration<WordReview>
{
    public void Configure(EntityTypeBuilder<WordReview> builder)
    {
        builder.ToTable("word_reviews");
        builder.HasKey(w => w.Id);

        builder.Property(w => w.RepetitionCount)
            .IsRequired();

        builder.Property(w => w.EaseFactor)
            .IsRequired();

        builder.Property(w => w.IntervalDays)
            .IsRequired();

        builder.Property(w => w.NextReviewAt)
            .IsRequired();

        builder.HasIndex(w => new { w.UserId, w.WordId }).IsUnique();
        builder.HasIndex(w => new { w.UserId, w.NextReviewAt });

        builder.Ignore(w => w.DomainEvents);
    }
}