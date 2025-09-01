using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Notifications;
using ZiyoNur.Domain.Entities.Orders;

namespace ZiyoNur.Domain.Entities.Users;

public class Seller : BaseAuditableEntity, IHasDomainEvent
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Role { get; set; } = "seller";

    public bool IsActive { get; set; } = true;

    [MaxLength(500)]
    public string? FcmToken { get; set; }

    public DateTime? LastLoginDate { get; set; }

    // Performance metrics
    public decimal TotalSalesAmount { get; set; } = 0;
    public int TotalOrdersProcessed { get; set; } = 0;

    // Full Name for display
    public string FullName => $"{FirstName} {LastName}";

    // Navigation Properties
    public virtual ICollection<Order> ProcessedOrders { get; set; } = new List<Order>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public void UpdateSalesMetrics()
    {
        TotalSalesAmount = ProcessedOrders
            .Where(o => o.Status != Enums.OrderStatus.Cancelled)
            .Sum(o => o.TotalPrice);

        TotalOrdersProcessed = ProcessedOrders
            .Count(o => o.Status != Enums.OrderStatus.Cancelled);
    }
}
