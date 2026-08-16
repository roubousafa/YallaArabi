using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class ChallengeParticipationConfiguration : IEntityTypeConfiguration<ChallengeParticipation>
{
    public void Configure(EntityTypeBuilder<ChallengeParticipation> builder)
    {
        builder.ToTable("challenge_participations");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(p => new { p.UserId, p.ChallengeId });
        builder.HasIndex(p => p.ChallengeId);

        builder.Ignore(p => p.DomainEvents);
    }
}