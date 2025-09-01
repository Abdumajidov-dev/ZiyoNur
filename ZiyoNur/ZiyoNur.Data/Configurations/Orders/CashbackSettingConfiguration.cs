using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Orders;

namespace ZiyoNur.Data.Configurations.Orders;

public class CashbackSettingConfiguration : IEntityTypeConfiguration<CashbackSetting>
{
    public void Configure(EntityTypeBuilder<CashbackSetting> builder)
    {
        builder.ToTable("CashbackSettings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.CashbackPercentage)
            .HasPrecision(5, 2)
            .HasDefaultValue(2.00m);

        builder.Property(x => x.ValidityPeriodDays)
            .HasDefaultValue(30);

        builder.Property(x => x.MinimumOrderAmount)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);

        builder.Property(x => x.IsActive)
            .HasDefaultValue(true);

        // Index on active status
        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_CashbackSettings_IsActive");
    }
}