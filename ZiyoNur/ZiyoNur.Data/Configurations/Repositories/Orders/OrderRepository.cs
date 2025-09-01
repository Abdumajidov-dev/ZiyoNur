using Microsoft.EntityFrameworkCore;
using ZiyoNur.Data.Common;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Enums;
using ZiyoNur.Domain.Repositories.Orders;

namespace ZiyoNur.Data.Configurations.Repositories.Orders;

public class OrderRepository : BaseRepository<Order>, IOrderRepository
{
    public OrderRepository(MarketplaceDbContext context) : base(context)
    {
    }

    public async Task<Order?> GetOrderWithItemsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<Order?> GetOrderWithDetailsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Seller)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                    .ThenInclude(p => p.Category)
            .Include(o => o.PaymentTransactions)
            .Include(o => o.Delivery)
                .ThenInclude(d => d!.DeliveryPartner)
            .Include(o => o.DiscountReason)
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<IReadOnlyList<Order>> GetCustomerOrdersAsync(int customerId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Order>> GetSellerOrdersAsync(int sellerId)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.SellerId == sellerId)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Seller)
            .Where(o => o.Status == status)
            .OrderBy(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Seller)
            .Where(o => o.OrderDate >= startDate && o.OrderDate <= endDate)
            .OrderBy(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Order>> GetPendingOrdersAsync()
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .Where(o => o.Status == OrderStatus.Pending)
            .OrderBy(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Order>> GetTodaysOrdersAsync()
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Seller)
            .Where(o => o.OrderDate >= today && o.OrderDate < tomorrow)
            .OrderByDescending(o => o.OrderDate)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        IQueryable<Order> query = _dbSet.Where(o => o.Status != OrderStatus.Cancelled);

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        return await query.SumAsync(o => o.TotalPrice - o.DiscountApplied);
    }

    public async Task<int> GetTotalOrdersCountAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        IQueryable<Order> query = _dbSet.Where(o => o.Status != OrderStatus.Cancelled);

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        return await query.CountAsync();
    }

    public async Task<decimal> GetCustomerTotalSpentAsync(int customerId)
    {
        return await _dbSet
            .Where(o => o.CustomerId == customerId && o.Status != OrderStatus.Cancelled)
            .SumAsync(o => o.TotalPrice - o.DiscountApplied);
    }

    public async Task<IReadOnlyList<Order>> GetRecentOrdersAsync(int count = 10)
    {
        return await _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Seller)
            .OrderByDescending(o => o.OrderDate)
            .Take(count)
            .ToListAsync();
    }

    public async Task<(IReadOnlyList<Order> Items, int TotalCount)> GetPagedOrdersAsync(
        int? customerId = null,
        int? sellerId = null,
        OrderStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        int pageIndex = 0,
        int pageSize = 20)
    {
        IQueryable<Order> query = _dbSet
            .Include(o => o.Customer)
            .Include(o => o.Seller);

        // Apply filters
        if (customerId.HasValue)
            query = query.Where(o => o.CustomerId == customerId.Value);

        if (sellerId.HasValue)
            query = query.Where(o => o.SellerId == sellerId.Value);

        if (status.HasValue)
            query = query.Where(o => o.Status == status.Value);

        if (startDate.HasValue)
            query = query.Where(o => o.OrderDate >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(o => o.OrderDate <= endDate.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(o => o.OrderDate)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
