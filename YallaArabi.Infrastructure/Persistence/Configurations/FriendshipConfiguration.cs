using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.ToTable("friendships");
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(f => new { f.RequesterId, f.AddresseeId }).IsUnique();
        builder.HasIndex(f => f.AddresseeId);
        builder.HasIndex(f => f.Status);

        builder.Ignore(f => f.DomainEvents);
    }
}