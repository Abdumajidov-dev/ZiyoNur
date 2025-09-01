using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Orders;

namespace ZiyoNur.Data.Configurations.Orders;

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
    {
        builder.ToTable("OrderStatusHistories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ChangedBy)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Notes)
            .HasMaxLength(1000);

        // Enum conversions
        builder.Property(x => x.OldStatus)
            .HasConversion<int>();

        builder.Property(x => x.NewStatus)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(x => x.Order)
            .WithMany(x => x.StatusHistory)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.OrderId)
            .HasDatabaseName("IX_OrderStatusHistories_OrderId");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_OrderStatusHistories_CreatedAt");

        builder.HasIndex(x => new { x.OrderId, x.CreatedAt })
            .HasDatabaseName("IX_OrderStatusHistories_Order_Date");
    }
}
