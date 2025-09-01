using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZiyoNur.Domain.Entities.Content;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ZiyoNur.Data.Configurations.Contents;

public class ContentConfiguration : IEntityTypeConfiguration<Content>
{
    public void Configure(EntityTypeBuilder<Content> builder)
    {
        builder.ToTable("Contents");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.ContentUrl)
            .HasMaxLength(500);

        builder.Property(x => x.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(x => x.TargetAudience)
            .HasMaxLength(20);

        builder.Property(x => x.Tags)
            .HasColumnType("nvarchar(max)");

        builder.Property(x => x.DisplayOrder)
            .HasDefaultValue(0);

        builder.Property(x => x.ViewCount)
            .HasDefaultValue(0);

        builder.Property(x => x.ClickCount)
            .HasDefaultValue(0);

        // Enum conversion
        builder.Property(x => x.Type)
            .HasConversion<int>();

        // Indexes
        builder.HasIndex(x => x.Type)
            .HasDatabaseName("IX_Contents_Type");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Contents_IsActive");

        builder.HasIndex(x => x.DisplayOrder)
            .HasDatabaseName("IX_Contents_DisplayOrder");

        builder.HasIndex(x => x.PublishDate)
            .HasDatabaseName("IX_Contents_PublishDate");

        builder.HasIndex(x => x.ExpiryDate)
            .HasDatabaseName("IX_Contents_ExpiryDate");

        builder.HasIndex(x => x.TargetAudience)
            .HasDatabaseName("IX_Contents_TargetAudience");

        // Ignore computed properties
        builder.Ignore(x => x.IsExpired);
        builder.Ignore(x => x.ShouldBeDisplayed);
        builder.Ignore(x => x.ClickThroughRate);
    }
}