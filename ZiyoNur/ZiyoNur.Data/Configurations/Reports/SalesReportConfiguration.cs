using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Reports;

namespace ZiyoNur.Data.Configurations.Reports;

public class SalesReportConfiguration : IEntityTypeConfiguration<SalesReport>
{
    public void Configure(EntityTypeBuilder<SalesReport> builder)
    {
        builder.ToTable("SalesReports");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ReportDate)
            .IsRequired();

        // All decimal properties with precision
        builder.Property(x => x.TotalSales)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.OnlineSales)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.OfflineSales)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalCashbackGiven)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalDiscountGiven)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.PaymeSales)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.UzcardSales)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.ClickSales)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.CashSales)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.CashbackSales)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        // Integer properties with defaults
        builder.Property(x => x.TotalOrders)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalCustomers)
            .HasDefaultValue(0);

        builder.Property(x => x.NewCustomers)
            .HasDefaultValue(0);

        builder.Property(x => x.ActiveCustomers)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalProductsSold)
            .HasDefaultValue(0);

        builder.Property(x => x.UniqueProductsSold)
            .HasDefaultValue(0);

        // Unique index on report date
        builder.HasIndex(x => x.ReportDate)
            .IsUnique()
            .HasDatabaseName("IX_SalesReports_ReportDate");

        // Ignore computed properties
        builder.Ignore(x => x.AverageOrderValue);
        builder.Ignore(x => x.OnlinePercentage);
        builder.Ignore(x => x.CustomerRetentionRate);
    }
}
