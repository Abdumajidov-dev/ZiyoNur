using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Products;

namespace ZiyoNur.Data.Configurations.Products;

public class ProductLikeConfiguration : IEntityTypeConfiguration<ProductLike>
{
    public void Configure(EntityTypeBuilder<ProductLike> builder)
    {
        builder.ToTable("ProductLikes");

        builder.HasKey(x => x.Id);

        // Relationships
        builder.HasOne(x => x.Customer)
            .WithMany(x => x.LikedProducts)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.Likes)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Composite unique index
        builder.HasIndex(x => new { x.CustomerId, x.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_ProductLikes_Customer_Product");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_ProductLikes_ProductId");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_ProductLikes_CreatedAt");
    }
}
