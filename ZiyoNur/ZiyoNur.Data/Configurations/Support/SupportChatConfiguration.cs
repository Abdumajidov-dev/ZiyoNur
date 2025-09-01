using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Support;

namespace ZiyoNur.Data.Configurations.Support;

public class SupportChatConfiguration : IEntityTypeConfiguration<SupportChat>
{
    public void Configure(EntityTypeBuilder<SupportChat> builder)
    {
        builder.ToTable("SupportChats");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Message)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.SenderType)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.FileUrl)
            .HasMaxLength(500);

        builder.Property(x => x.FileName)
            .HasMaxLength(100);

        builder.Property(x => x.FileType)
            .HasMaxLength(50);

        // Enum conversion
        builder.Property(x => x.MessageType)
            .HasConversion<int>();

        // Relationships
        builder.HasOne(x => x.Customer)
            .WithMany(x => x.SupportChats)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Admin)
            .WithMany(x => x.SupportChats)
            .HasForeignKey(x => x.AdminId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        builder.HasIndex(x => x.CustomerId)
            .HasDatabaseName("IX_SupportChats_CustomerId");

        builder.HasIndex(x => x.AdminId)
            .HasDatabaseName("IX_SupportChats_AdminId");

        builder.HasIndex(x => x.SenderType)
            .HasDatabaseName("IX_SupportChats_SenderType");

        builder.HasIndex(x => x.IsRead)
            .HasDatabaseName("IX_SupportChats_IsRead");

        builder.HasIndex(x => x.CreatedAt)
            .HasDatabaseName("IX_SupportChats_CreatedAt");

        // Composite index for chat threads
        builder.HasIndex(x => new { x.CustomerId, x.CreatedAt })
            .HasDatabaseName("IX_SupportChats_Customer_Date");

        // Ignore Domain Events and computed properties
        builder.Ignore(x => x.DomainEvents);
        builder.Ignore(x => x.IsSentByCustomer);
        builder.Ignore(x => x.IsSentByAdmin);
        builder.Ignore(x => x.HasAttachment);
    }
}