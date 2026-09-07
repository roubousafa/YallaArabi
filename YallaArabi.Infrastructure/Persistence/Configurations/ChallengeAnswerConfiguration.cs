using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class ChallengeAnswerConfiguration : IEntityTypeConfiguration<ChallengeAnswer>
{
    public void Configure(EntityTypeBuilder<ChallengeAnswer> builder)
    {
        builder.ToTable("challenge_answers");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.SelectedOption).HasMaxLength(1);
        builder.Property(a => a.CorrectOption).HasMaxLength(1).IsRequired();

        builder.HasIndex(a => a.ParticipationId);
    }
}