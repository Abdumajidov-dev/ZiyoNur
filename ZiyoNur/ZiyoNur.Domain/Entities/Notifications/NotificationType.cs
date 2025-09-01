using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Notifications;

public class NotificationType : BaseAuditableEntity
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty; // order_created, promotion, cashback_earned, news

    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Template for notifications
    [MaxLength(200)]
    public string? DefaultTitle { get; set; }
    public string? DefaultMessage { get; set; }

    // Settings
    public bool SendPushNotification { get; set; } = true;
    public bool SendInApp { get; set; } = true;
    public bool SendEmail { get; set; } = false;
    public bool SendSms { get; set; } = false;

    // Statistics
    public int TotalSent { get; set; } = 0;
    public int TotalRead { get; set; } = 0;

    // Navigation Properties
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    // Business Methods
    public decimal ReadRate => TotalSent > 0 ? (decimal)TotalRead / TotalSent * 100 : 0;

    public void IncrementStats(bool wasRead = false)
    {
        TotalSent++;
        if (wasRead) TotalRead++;
    }
}
