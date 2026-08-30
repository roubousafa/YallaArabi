using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class ChallengeAssignmentConfiguration : IEntityTypeConfiguration<ChallengeAssignment>
{
    public void Configure(EntityTypeBuilder<ChallengeAssignment> builder)
    {
        builder.ToTable("challenge_assignments");
        builder.HasKey(a => a.Id);

        builder.HasIndex(a => new { a.ChallengeId, a.UserId }).IsUnique();
        builder.HasIndex(a => a.UserId);

        builder.Ignore(a => a.DomainEvents);
    }
}