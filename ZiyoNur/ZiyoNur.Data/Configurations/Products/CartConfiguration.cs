using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Products;

namespace ZiyoNur.Data.Configurations.Products;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasDefaultValue(1);

        // Relationships
        builder.HasOne(x => x.Customer)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.CartItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Composite unique index
        builder.HasIndex(x => new { x.CustomerId, x.ProductId })
            .IsUnique()
            .HasDatabaseName("IX_Carts_Customer_Product");

        builder.HasIndex(x => x.CustomerId)
            .HasDatabaseName("IX_Carts_CustomerId");

        // Ignore computed properties
        builder.Ignore(x => x.TotalPrice);
    }
}
