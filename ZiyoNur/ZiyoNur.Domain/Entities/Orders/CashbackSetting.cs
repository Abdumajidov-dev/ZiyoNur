using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Entities.Orders;

public class CashbackSetting : BaseAuditableEntity
{
    [Range(0, 100)]
    public decimal CashbackPercentage { get; set; } = 2.00m;

    public int ValidityPeriodDays { get; set; } = 30;
    public decimal MinimumOrderAmount { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Business Methods
    public decimal CalculateCashback(decimal orderAmount)
    {
        if (!IsActive || orderAmount < MinimumOrderAmount)
            return 0;

        return orderAmount * (CashbackPercentage / 100);
    }

    public DateTime GetExpiryDate()
    {
        return DateTime.UtcNow.AddDays(ValidityPeriodDays);
    }
}
