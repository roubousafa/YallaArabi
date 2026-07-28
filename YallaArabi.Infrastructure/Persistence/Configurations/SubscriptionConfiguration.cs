using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.PlanCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(s => s.PaymentProviderReference)
            .HasMaxLength(200);

        builder.HasIndex(s => s.UserId);

        builder.Ignore(s => s.DomainEvents);
    }
}