using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Delivery;

namespace ZiyoNur.Data.Configurations.Delivery;

public class OrderDeliveryConfiguration : IEntityTypeConfiguration<OrderDelivery>
{
    public void Configure(EntityTypeBuilder<OrderDelivery> builder)
    {
        builder.ToTable("OrderDeliveries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TrackingCode)
            .HasMaxLength(100);

        builder.Property(x => x.DeliveryAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.DeliveryFee)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.DeliveryNotes)
            .HasMaxLength(1000);

        builder.Property(x => x.RecipientName)
            .HasMaxLength(100);

        builder.Property(x => x.RecipientPhone)
            .HasMaxLength(20);

        // Enum conversion
        builder.Property(x => x.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(x => x.Order)
            .WithOne(x => x.Delivery)
            .HasForeignKey<OrderDelivery>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.DeliveryPartner)
            .WithMany(x => x.OrderDeliveries)
            .HasForeignKey(x => x.DeliveryPartnerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.OrderId)
            .IsUnique()
            .HasDatabaseName("IX_OrderDeliveries_OrderId");

        builder.HasIndex(x => x.TrackingCode)
            .IsUnique()
            .HasDatabaseName("IX_OrderDeliveries_TrackingCode")
            .HasFilter("[TrackingCode] IS NOT NULL");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_OrderDeliveries_Status");

        builder.HasIndex(x => x.DeliveryPartnerId)
            .HasDatabaseName("IX_OrderDeliveries_DeliveryPartnerId");

        builder.HasIndex(x => x.EstimatedDeliveryDate)
            .HasDatabaseName("IX_OrderDeliveries_EstimatedDeliveryDate");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.IsDelivered);
        builder.Ignore(x => x.IsInProgress);
        builder.Ignore(x => x.IsOverdue);
    }
}
