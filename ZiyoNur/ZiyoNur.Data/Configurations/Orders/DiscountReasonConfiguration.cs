using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Orders;

namespace ZiyoNur.Data.Configurations.Orders;

public class DiscountReasonConfiguration : IEntityTypeConfiguration<DiscountReason>
{
    public void Configure(EntityTypeBuilder<DiscountReason> builder)
    {
        builder.ToTable("DiscountReasons");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.UsageCount)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalDiscountGiven)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_DiscountReasons_IsActive");

        builder.HasIndex(x => x.Name)
            .HasDatabaseName("IX_DiscountReasons_Name");
    }
}
