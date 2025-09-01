using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Repositories.Orders;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<Order?> GetOrderWithItemsAsync(int orderId);
    Task<Order?> GetOrderWithDetailsAsync(int orderId); // includes customer, seller, items, payments
    Task<IReadOnlyList<Order>> GetCustomerOrdersAsync(int customerId);
    Task<IReadOnlyList<Order>> GetSellerOrdersAsync(int sellerId);
    Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(OrderStatus status);
    Task<IReadOnlyList<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<Order>> GetPendingOrdersAsync();
    Task<IReadOnlyList<Order>> GetTodaysOrdersAsync();
    Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<int> GetTotalOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null);
    Task<decimal> GetCustomerTotalSpentAsync(int customerId);
    Task<IReadOnlyList<Order>> GetRecentOrdersAsync(int count = 10);
    Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedOrdersAsync(
        int? customerId = null,
        int? sellerId = null,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageIndex = 0,
        int pageSize = 20);
}
