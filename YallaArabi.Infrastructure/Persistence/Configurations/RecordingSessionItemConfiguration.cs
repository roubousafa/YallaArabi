using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class RecordingSessionItemConfiguration : IEntityTypeConfiguration<RecordingSessionItem>
{
    public void Configure(EntityTypeBuilder<RecordingSessionItem> builder)
    {
        builder.ToTable("recording_session_items");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.TextToRead).HasMaxLength(500).IsRequired();
        builder.Property(i => i.ClipUrl).HasMaxLength(500);
        builder.Property(i => i.Status).HasMaxLength(20).IsRequired();
        builder.Property(i => i.FlagReason).HasMaxLength(300);

        builder.HasIndex(i => new { i.SessionId, i.OrderNumber }).IsUnique();
        builder.HasIndex(i => i.Status);
    }
}