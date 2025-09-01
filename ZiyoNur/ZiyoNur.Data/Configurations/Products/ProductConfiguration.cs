using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Products;

namespace ZiyoNur.Data.Configurations.Products;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.Count)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(x => x.QrCode)
            .HasMaxLength(100);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.SearchKeywords)
            .HasMaxLength(500);

        builder.Property(x => x.Barcode)
            .HasMaxLength(200);

        builder.Property(x => x.AverageRating)
            .HasPrecision(3, 2)
            .HasDefaultValue(0);

        // Enum conversion
        builder.Property(x => x.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.CategoryId)
            .HasDatabaseName("IX_Products_CategoryId");

        builder.HasIndex(x => x.QrCode)
            .IsUnique()
            .HasDatabaseName("IX_Products_QrCode")
            .HasFilter("[QrCode] IS NOT NULL");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Products_Status");

        builder.HasIndex(x => x.Price)
            .HasDatabaseName("IX_Products_Price");

        builder.HasIndex(x => x.Count)
            .HasDatabaseName("IX_Products_Count");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_Products_CreatedAt");

        // Full-text search index (if using SQL Server)
        builder.HasIndex(x => new { x.Name, x.Description, x.SearchKeywords })
            .HasDatabaseName("IX_Products_FullText");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.IsAvailable);
        builder.Ignore(x => x.IsOutOfStock);
        builder.Ignore(x => x.StatusText);
    }
}
