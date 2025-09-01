using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Products;

namespace ZiyoNur.Data.Configurations.Products;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.SortOrder)
            .HasDefaultValue(0);

        // Self-referencing relationship
        builder.HasOne(x => x.Parent)
            .WithMany(x => x.SubCategories)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.ParentId)
            .HasDatabaseName("IX_Categories_ParentId");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Categories_IsActive");

        builder.HasIndex(x => x.SortOrder)
            .HasDatabaseName("IX_Categories_SortOrder");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.IsSubCategory);
        builder.Ignore(x => x.HasProducts);
        builder.Ignore(x => x.HasSubCategories);
    }
}
