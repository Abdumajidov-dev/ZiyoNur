using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Notifications;

namespace ZiyoNur.Data.Configurations.Notifications;

public class AdminNotificationCampaignConfiguration : IEntityTypeConfiguration<AdminNotificationCampaign>
{
    public void Configure(EntityTypeBuilder<AdminNotificationCampaign> builder)
    {
        builder.ToTable("AdminNotificationCampaigns");

        builder.HasKey(x => x.Id);

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

        builder.Property(x => x.TargetUserType)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("customer");

        builder.Property(x => x.TargetCustomerIds)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.TotalRecipients)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalSent)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalDelivered)
            .HasDefaultValue(0);

        builder.Property(x => x.TotalRead)
            .HasDefaultValue(0);

        // Enum conversion
        builder.Property(x => x.Status)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(x => x.Admin)
            .WithMany(x => x.NotificationCampaigns)
            .HasForeignKey(x => x.AdminId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(x => x.AdminId)
            .HasDatabaseName("IX_AdminNotificationCampaigns_AdminId");

        builder.HasIndex(x => x.Status)
            .HasDatabaseName("IX_AdminNotificationCampaigns_Status");

        builder.HasIndex(x => x.ScheduledAt)
            .HasDatabaseName("IX_AdminNotificationCampaigns_ScheduledAt");

        builder.HasIndex(x => x.TargetUserType)
            .HasDatabaseName("IX_AdminNotificationCampaigns_TargetUserType");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.CanBeSent);
        builder.Ignore(x => x.IsScheduled);
        builder.Ignore(x => x.ShouldBeSentNow);
        builder.Ignore(x => x.DeliveryRate);
        builder.Ignore(x => x.ReadRate);
    }
}