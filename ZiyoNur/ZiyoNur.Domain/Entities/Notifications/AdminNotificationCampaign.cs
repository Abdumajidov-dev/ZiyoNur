using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Notifications;

public class AdminNotificationCampaign : BaseAuditableEntity, IHasDomainEvent
{
    public int AdminId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Message { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [MaxLength(500)]
    public string? ActionUrl { get; set; }

    [MaxLength(20)]
    public string TargetUserType { get; set; } = "customer"; // customer, seller, all

    public string? TargetCustomerIds { get; set; } // JSON array, specific customers

    // Scheduling
    public DateTime? ScheduledAt { get; set; } // keyinchalik jo'natish uchun
    public DateTime? SentAt { get; set; } // qachon jo'natilgan

    public CampaignStatus Status { get; set; } = CampaignStatus.Draft;

    // Statistics
    public int TotalRecipients { get; set; } = 0;
    public int TotalSent { get; set; } = 0;
    public int TotalDelivered { get; set; } = 0;
    public int TotalRead { get; set; } = 0;

    // Campaign settings
    public bool SendPushNotification { get; set; } = true;
    public bool SendInApp { get; set; } = true;
    public bool SendEmail { get; set; } = false;
    public bool SendSms { get; set; } = false;

    // Navigation Properties
    public virtual Admin Admin { get; set; } = null!;

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public bool CanBeSent => Status == CampaignStatus.Draft || Status == CampaignStatus.Scheduled;
    public bool IsScheduled => ScheduledAt.HasValue && ScheduledAt > DateTime.UtcNow;
    public bool ShouldBeSentNow => CanBeSent &&
        (!ScheduledAt.HasValue || ScheduledAt <= DateTime.UtcNow);

    public decimal DeliveryRate => TotalSent > 0 ? (decimal)TotalDelivered / TotalSent * 100 : 0;
    public decimal ReadRate => TotalDelivered > 0 ? (decimal)TotalRead / TotalDelivered * 100 : 0;

    public void Schedule(DateTime scheduleTime)
    {
        if (scheduleTime <= DateTime.UtcNow)
            throw new ArgumentException("Schedule time must be in the future");

        ScheduledAt = scheduleTime;
        Status = CampaignStatus.Scheduled;

        DomainEvents.Add(new Events.Notifications.CampaignScheduledEvent(
            Id, AdminId, scheduleTime));
    }

    public void Send()
    {
        if (!CanBeSent)
            throw new InvalidOperationException("Campaign cannot be sent");

        Status = CampaignStatus.Sent;
        SentAt = DateTime.UtcNow;

        DomainEvents.Add(new Events.Notifications.CampaignSentEvent(
            Id, AdminId, TotalRecipients));
    }

    public void Cancel(string reason = "Cancelled by admin")
    {
        if (Status == CampaignStatus.Sent)
            throw new InvalidOperationException("Cannot cancel sent campaign");

        Status = CampaignStatus.Cancelled;

        DomainEvents.Add(new Events.Notifications.CampaignCancelledEvent(
            Id, AdminId, reason));
    }

    public void UpdateStats(int sent, int delivered, int read)
    {
        TotalSent += sent;
        TotalDelivered += delivered;
        TotalRead += read;
    }
}
