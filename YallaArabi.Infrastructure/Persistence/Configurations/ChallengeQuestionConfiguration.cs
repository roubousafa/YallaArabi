using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class ChallengeQuestionConfiguration : IEntityTypeConfiguration<ChallengeQuestion>
{
    public void Configure(EntityTypeBuilder<ChallengeQuestion> builder)
    {
        builder.ToTable("challenge_questions");
        builder.HasKey(q => q.Id);

        builder.Property(q => q.QuestionText)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(q => q.OptionA)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(q => q.OptionB)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(q => q.OptionC)
            .HasMaxLength(200);

        builder.Property(q => q.OptionD)
            .HasMaxLength(200);

        builder.Property(q => q.CorrectOption)
            .HasMaxLength(1)
            .IsRequired();

        builder.Property(q => q.AudioUrl)
           .HasMaxLength(500);

        builder.Property(q => q.ImageUrl)
            .HasMaxLength(500);

        builder.HasIndex(q => q.ChallengeId);
    }
}