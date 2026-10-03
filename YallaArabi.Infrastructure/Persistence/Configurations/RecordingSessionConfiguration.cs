using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class RecordingSessionConfiguration : IEntityTypeConfiguration<RecordingSession>
{
    public void Configure(EntityTypeBuilder<RecordingSession> builder)
    {
        builder.ToTable("recording_sessions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Dialect).HasMaxLength(30).IsRequired();
        builder.Property(s => s.Gender).HasMaxLength(10).IsRequired();
        builder.Property(s => s.Status).HasMaxLength(20).IsRequired();
        builder.Property(s => s.UploadedAudioUrl).HasMaxLength(500);

        builder.HasIndex(s => s.Status);

        builder.Ignore(s => s.DomainEvents);
    }
}