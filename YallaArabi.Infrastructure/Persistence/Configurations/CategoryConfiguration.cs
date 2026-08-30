using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YallaArabi.Domain.Entities;

namespace YallaArabi.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(c => c.OrderIndex)
            .IsRequired();

        builder.HasIndex(c => c.LevelId);

        builder.Property(c => c.Description)
           .HasMaxLength(300);

        builder.Property(c => c.IconUrl)
            .HasMaxLength(500);
    }
}