namespace ZiyoNur.Service.DTOs.Reports;

public class SalesReportDto
{
    public DateTime ReportDate { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal OnlineSales { get; set; }
    public decimal OfflineSales { get; set; }
    public int TotalCustomers { get; set; }
    public int NewCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public decimal TotalCashbackGiven { get; set; }
    public decimal TotalDiscountGiven { get; set; }
    public int TotalProductsSold { get; set; }
    public int UniqueProductsSold { get; set; }
    public decimal AverageOrderValue { get; set; }
    public decimal OnlinePercentage { get; set; }
    public decimal CustomerRetentionRate { get; set; }

    // Payment method breakdown
    public decimal PaymeSales { get; set; }
    public decimal UzcardSales { get; set; }
    public decimal ClickSales { get; set; }
    public decimal CashSales { get; set; }
    public decimal CashbackSales { get; set; }

    // Top performing products
    public List<TopProductDto> TopProducts { get; set; } = new();

    // Top customers
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
}

public class TopProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal Revenue { get; set; }
}

public class TopCustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalSpent { get; set; }
    public int OrderCount { get; set; }
}