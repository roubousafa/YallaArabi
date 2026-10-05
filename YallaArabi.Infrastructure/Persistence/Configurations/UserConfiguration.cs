using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;
using YallaArabi.Domain.ValueObjects;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(u => u.Email)
            .HasConversion(
                email => email.Value,
                value => Email.Create(value))
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.PasswordHash);

        builder.Property(u => u.SubscriptionStatus)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(u => u.PreferredDialect)
    .HasMaxLength(30);

        builder.Property(u => u.PreferredVoiceGender)
    .HasMaxLength(10);
        builder.Property(u => u.AvatarUrl)
    .HasMaxLength(500);
        builder.Property(u => u.FriendCode)
    .HasMaxLength(10);

        builder.HasIndex(u => u.FriendCode).IsUnique();

        builder.Ignore(u => u.DomainEvents);
    }
}