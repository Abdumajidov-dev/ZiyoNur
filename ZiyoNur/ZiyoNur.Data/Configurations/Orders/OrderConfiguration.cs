using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Orders;

namespace ZiyoNur.Data.Configurations.Orders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderDate)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(x => x.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.DiscountApplied)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.PickupLocation)
            .HasMaxLength(100)
            .HasDefaultValue("main_library");

        builder.Property(x => x.OrderSource)
            .HasMaxLength(20)
            .HasDefaultValue("mobile");

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        // Enum conversions
        builder.Property(x => x.PaymentMethod)
            .HasConversion<int>();

        builder.Property(x => x.PaymentStatus)
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.DeliveryType)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(x => x.Customer)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Seller)
            .WithMany(x => x.ProcessedOrders)
            .HasForeignKey(x => x.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.DiscountReason)
            .WithMany(x => x.Orders)
            .HasForeignKey(x => x.DiscountReasonId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(x => x.CustomerId)
            .HasDatabaseName("IX_Orders_CustomerId");

        builder.HasIndex(x => x.SellerId)
            .HasDatabaseName("IX_Orders_SellerId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_Orders_Status");

        builder.HasIndex(x => x.PaymentStatus)
            .HasDatabaseName("IX_Orders_PaymentStatus");

        builder.HasIndex(x => x.OrderDate)
            .HasDatabaseName("IX_Orders_OrderDate");

        builder.HasIndex(x => x.DeliveryType)
            .HasDatabaseName("IX_Orders_DeliveryType");

        // Composite indexes for common queries
        builder.HasIndex(x => new { x.CustomerId, x.Status })
            .HasDatabaseName("IX_Orders_Customer_Status");

        builder.HasIndex(x => new { x.OrderDate, x.Status })
            .HasDatabaseName("IX_Orders_Date_Status");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.FinalPrice);
        builder.Ignore(x => x.IsOnlineOrder);
        builder.Ignore(x => x.IsCompleted);
        builder.Ignore(x => x.CanBeCancelled);
    }
}
