using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Orders;

public class CashbackTransaction : BaseAuditableEntity
{
    public int CustomerId { get; set; }
    public int OrderId { get; set; }

    public decimal Amount { get; set; }
    public CashbackTransactionType Type { get; set; }

    public DateTime? ExpiryDate { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime? UsedDate { get; set; }

    // Navigation Properties
    public virtual Customer Customer { get; set; } = null!;
    public virtual Order Order { get; set; } = null!;

    // Business Methods
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate < DateTime.UtcNow;
    public bool CanBeUsed => !IsUsed && !IsExpired && Type == CashbackTransactionType.Earned;

    public void MarkAsUsed()
    {
        if (!CanBeUsed)
            throw new InvalidOperationException("Cashback cannot be used");

        IsUsed = true;
        UsedDate = DateTime.UtcNow;
    }
}
