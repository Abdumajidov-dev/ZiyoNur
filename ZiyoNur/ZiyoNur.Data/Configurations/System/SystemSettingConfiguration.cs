using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.System;

namespace ZiyoNur.Data.Configurations.System;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.SettingKey)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.SettingValue)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.DataType)
            .HasMaxLength(20)
            .HasDefaultValue("string");

        builder.Property(x => x.IsPublic)
            .HasDefaultValue(false);

        // Unique index on setting key
        builder.HasIndex(x => x.SettingKey)
            .IsUnique()
            .HasDatabaseName("IX_SystemSettings_SettingKey");

        builder.HasIndex(x => x.IsPublic)
            .HasDatabaseName("IX_SystemSettings_IsPublic");
    }
}