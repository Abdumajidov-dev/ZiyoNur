using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Notifications;
using ZiyoNur.Domain.Entities.Support;

namespace ZiyoNur.Domain.Entities.Users;

public class Admin : BaseAuditableEntity, IHasDomainEvent
{
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = "admin";

    [MaxLength(20)]
    public string? Phone { get; set; }

    [MaxLength(100)]
    public string? Email { get; set; }

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? FcmToken { get; set; }

    public DateTime? LastLoginDate { get; set; }

    // Permission flags
    public bool CanManageProducts { get; set; } = true;
    public bool CanManageOrders { get; set; } = true;
    public bool CanManageUsers { get; set; } = true;
    public bool CanViewReports { get; set; } = true;
    public bool CanManageNotifications { get; set; } = true;

    // Full Name for display
    public string FullName => $"{FirstName} {LastName}";

    // Navigation Properties
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<AdminNotificationCampaign> NotificationCampaigns { get; set; } = new List<AdminNotificationCampaign>();
    public virtual ICollection<SupportChat> SupportChats { get; set; } = new List<SupportChat>();

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public bool HasPermission(string permission)
    {
        return permission switch
        {
            "manage_products" => CanManageProducts,
            "manage_orders" => CanManageOrders,
            "manage_users" => CanManageUsers,
            "view_reports" => CanViewReports,
            "manage_notifications" => CanManageNotifications,
            _ => false
        };
    }
}
