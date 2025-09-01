using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Orders;

namespace ZiyoNur.Data.Configurations.Orders;

public class CashbackTransactionConfiguration : IEntityTypeConfiguration<CashbackTransaction>
{
    public void Configure(EntityTypeBuilder<CashbackTransaction> builder)
    {
        builder.ToTable("CashbackTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        // Enum conversion
        builder.Property(x => x.Type)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(x => x.Customer)
            .WithMany(x => x.CashbackTransactions)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Order)
            .WithMany(x => x.CashbackTransactions)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.CustomerId)
            .HasDatabaseName("IX_CashbackTransactions_CustomerId");

        builder.HasIndex(x => x.OrderId)
            .HasDatabaseName("IX_CashbackTransactions_OrderId");

        builder.HasIndex(x => x.Type)
            .HasDatabaseName("IX_CashbackTransactions_Type");

        builder.HasIndex(x => x.ExpiryDate)
            .HasDatabaseName("IX_CashbackTransactions_ExpiryDate");

        builder.HasIndex(x => new { x.CustomerId, x.IsUsed, x.Type })
            .HasDatabaseName("IX_CashbackTransactions_Customer_Used_Type");

        // Ignore computed properties
        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.CanBeUsed);
    }
}
