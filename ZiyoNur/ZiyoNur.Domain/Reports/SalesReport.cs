using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Reports;

public class SalesReport : BaseAuditableEntity
{
    public DateTime ReportDate { get; set; }

    public decimal TotalSales { get; set; } = 0;
    public int TotalOrders { get; set; } = 0;
    public decimal OnlineSales { get; set; } = 0;
    public decimal OfflineSales { get; set; } = 0;

    public int TotalCustomers { get; set; } = 0;
    public int NewCustomers { get; set; } = 0;
    public int ActiveCustomers { get; set; } = 0;

    public decimal TotalCashbackGiven { get; set; } = 0;
    public decimal TotalDiscountGiven { get; set; } = 0;

    // Product metrics
    public int TotalProductsSold { get; set; } = 0;
    public int UniqueProductsSold { get; set; } = 0;

    // Payment method breakdown
    public decimal PaymeSales { get; set; } = 0;
    public decimal UzcardSales { get; set; } = 0;
    public decimal ClickSales { get; set; } = 0;
    public decimal CashSales { get; set; } = 0;
    public decimal CashbackSales { get; set; } = 0;

    // Business Methods
    public decimal AverageOrderValue => TotalOrders > 0 ? TotalSales / TotalOrders : 0;
    public decimal OnlinePercentage => TotalSales > 0 ? (OnlineSales / TotalSales) * 100 : 0;
    public decimal CustomerRetentionRate => TotalCustomers > 0 ?
        ((decimal)(TotalCustomers - NewCustomers) / TotalCustomers) * 100 : 0;
}
