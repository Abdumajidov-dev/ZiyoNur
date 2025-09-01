using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Content;

public class Content : BaseAuditableEntity
{
    public ContentType Type { get; set; } // text, video, banner, promotion

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ContentUrl { get; set; }

    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;

    // Additional properties for different content types
    [MaxLength(500)]
    public string? ThumbnailUrl { get; set; }
    public DateTime? PublishDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    // Targeting
    [MaxLength(20)]
    public string? TargetAudience { get; set; } // all, customer, seller
    public string? Tags { get; set; } // JSON array

    // Metrics
    public int ViewCount { get; set; } = 0;
    public int ClickCount { get; set; } = 0;

    // Business Methods
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate < DateTime.UtcNow;
    public bool ShouldBeDisplayed => IsActive && !IsExpired;
    public decimal ClickThroughRate => ViewCount > 0 ? (decimal)ClickCount / ViewCount * 100 : 0;

    public void IncrementViewCount()
    {
        ViewCount++;
    }

    public void IncrementClickCount()
    {
        ClickCount++;
    }
}
