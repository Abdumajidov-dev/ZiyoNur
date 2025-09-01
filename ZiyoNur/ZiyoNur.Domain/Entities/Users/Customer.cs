using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Notifications;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Entities.Support;

namespace ZiyoNur.Domain.Entities.Users;

public class Customer : BaseAuditableEntity, IHasDomainEvent
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

    [MaxLength(100)]
    public string? Email { get; set; }

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;

    public decimal TotalCashback { get; set; } = 0;

    [MaxLength(500)]
    public string? FcmToken { get; set; }

    public DateTime? LastLoginDate { get; set; }

    // Full Name for display
    public string FullName => $"{FirstName} {LastName}";

    // Navigation Properties
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
    public virtual ICollection<ProductLike> LikedProducts { get; set; } = new List<ProductLike>();
    public virtual ICollection<CashbackTransaction> CashbackTransactions { get; set; } = new List<CashbackTransaction>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<SupportChat> SupportChats { get; set; } = new List<SupportChat>();

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public void UpdateCashbackBalance()
    {
        TotalCashback = CashbackTransactions
            .Where(c => c.Type == Enums.CashbackTransactionType.Earned && !c.IsUsed)
            .Sum(c => c.Amount);
    }

    public bool CanUseCashback(decimal amount)
    {
        return TotalCashback >= amount && amount > 0;
    }

    public void AddToCart(int productId, int quantity)
    {
        var existingItem = CartItems.FirstOrDefault(c => c.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            CartItems.Add(new Cart
            {
                CustomerId = Id,
                ProductId = productId,
                Quantity = quantity
            });
        }
    }
}
