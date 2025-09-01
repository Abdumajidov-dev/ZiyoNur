using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Notifications;

namespace ZiyoNur.Data.Configurations.Notifications;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ActionUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ExtraData)
            .HasColumnType("nvarchar(max)");

        // Relationships
        builder.HasOne(x => x.NotificationType)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.NotificationTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => new { x.UserId, x.UserType })
            .HasDatabaseName("IX_Notifications_User");

        builder.HasIndex(x => x.IsRead)
            .HasDatabaseName("IX_Notifications_IsRead");

        builder.HasIndex(x => x.IsSent)
            .HasDatabaseName("IX_Notifications_IsSent");

        builder.HasIndex(x => x.NotificationTypeId)
            .HasDatabaseName("IX_Notifications_NotificationTypeId");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_Notifications_CreatedAt");

        // Composite index for common queries
        builder.HasIndex(x => new { x.UserId, x.UserType, x.IsRead })
            .HasDatabaseName("IX_Notifications_User_Read");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.ShouldBeSent);
    }
}
