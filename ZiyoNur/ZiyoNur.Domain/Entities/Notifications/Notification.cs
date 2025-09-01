using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Notifications;

public class Notification : BaseAuditableEntity, IHasDomainEvent
{
    public int UserId { get; set; }

    [MaxLength(20)]
    public string UserType { get; set; } = string.Empty; // customer, admin, seller

    public int NotificationTypeId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(500)]
    public string? ActionUrl { get; set; } // bosilganda qayerga yo'nalish

    public bool IsRead { get; set; } = false;
    public bool IsSent { get; set; } = false; // push notification yuborilganmi

    public DateTime? ReadAt { get; set; }
    public DateTime? SentAt { get; set; }

    // Additional metadata
    public string? ExtraData { get; set; } // JSON format for additional data

    // Navigation Properties
    public virtual NotificationType NotificationType { get; set; } = null!;

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;

            DomainEvents.Add(new Events.Notifications.NotificationReadEvent(
                Id, UserId, UserType));
        }
    }

    public void MarkAsSent()
    {
        if (!IsSent)
        {
            IsSent = true;
            SentAt = DateTime.UtcNow;
        }
    }

    public bool ShouldBeSent => !IsSent && NotificationType.IsActive;
}
