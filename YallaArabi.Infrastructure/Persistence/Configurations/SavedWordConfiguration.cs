using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class SavedWordConfiguration : IEntityTypeConfiguration<SavedWord>
{
    public void Configure(EntityTypeBuilder<SavedWord> builder)
    {
        builder.ToTable("saved_words");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.SavedAt)
            .IsRequired();

        builder.HasIndex(s => new { s.UserId, s.WordId }).IsUnique();

        builder.Ignore(s => s.DomainEvents);
    }
}