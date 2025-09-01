using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Orders;

namespace ZiyoNur.Data.Configurations.Orders;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountApplied)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        // Relationships
        builder.HasOne(x => x.Order)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Product)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.OrderId)
            .HasDatabaseName("IX_OrderItems_OrderId");

        builder.HasIndex(x => x.ProductId)
            .HasDatabaseName("IX_OrderItems_ProductId");

        // Ignore computed properties
        builder.Ignore(x => x.FinalPrice);
        builder.Ignore(x => x.DiscountPercentage);
    }
}
