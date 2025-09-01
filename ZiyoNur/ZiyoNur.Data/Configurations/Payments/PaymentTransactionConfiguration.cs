using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Payments;

namespace ZiyoNur.Data.Configurations.Payments;

public class PaymentTransactionConfiguration : IEntityTypeConfiguration<PaymentTransaction>
{
    public void Configure(EntityTypeBuilder<PaymentTransaction> builder)
    {
        builder.ToTable("PaymentTransactions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.PaymentMethod)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.PaymentProvider)
            .HasMaxLength(50);

        builder.Property(x => x.TransactionId)
            .HasMaxLength(200);

        builder.Property(x => x.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(x => x.CashbackUsed)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.CashAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.CardMask)
            .HasMaxLength(20);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.Property(x => x.ProviderResponse)
            .HasColumnType("nvarchar(max)");

        // Enum conversion
        builder.Property(x => x.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(x => x.Order)
            .WithMany(x => x.PaymentTransactions)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(x => x.OrderId)
            .HasDatabaseName("IX_PaymentTransactions_OrderId");

        builder.HasIndex(x => x.TransactionId)
            .IsUnique()
            .HasDatabaseName("IX_PaymentTransactions_TransactionId")
            .HasFilter("[TransactionId] IS NOT NULL");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_PaymentTransactions_Status");

        builder.HasIndex(x => x.PaymentMethod)
            .HasDatabaseName("IX_PaymentTransactions_PaymentMethod");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_PaymentTransactions_CreatedAt");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.IsSuccessful);
        builder.Ignore(x => x.IsPending);
        builder.Ignore(x => x.IsFailed);
    }
}
