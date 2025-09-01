using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Notifications;

namespace ZiyoNur.Data.Configurations.Notifications;

public class NotificationTypeConfiguration : IEntityTypeConfiguration<NotificationType>
{
    public void Configure(EntityTypeBuilder<NotificationType> builder)
    {
        builder.ToTable("NotificationTypes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.DefaultTitle)
            .HasMaxLength(200);

        builder.Property(x => x.DefaultMessage)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.TotalSent)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalRead)
            .HasDefaultValue(0);

        // Indexes
        builder.HasIndex(x => x.Name)
            .IsUnique()
            .HasDatabaseName("IX_NotificationTypes_Name");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_NotificationTypes_IsActive");

        // Ignore computed properties
        builder.Ignore(x => x.ReadRate);
    }
}
