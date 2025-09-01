using Microsoft.EntityFrameworkCore;
using ZiyoNur.Data.Common;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Repositories.Users;

namespace ZiyoNur.Data.Configurations.Repositories.Users;

public class SellerRepository : BaseRepository<Seller>, ISellerRepository
{
    public SellerRepository(MarketplaceDbContext context) : base(context)
    {
    }

    public async Task<Seller?> GetByPhoneAsync(string phone)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Phone == phone);
    }

    public async Task<Seller?> GetSellerWithOrdersAsync(int sellerId)
    {
        return await _dbSet
            .Include(x => x.ProcessedOrders)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(x => x.Id == sellerId);
    }

    public async Task<IReadOnlyList<Seller>> GetActiveSellerAsync()
    {
        return await _dbSet
            .Where(x => x.IsActive)
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync();
    }

    public async Task<decimal> GetSellerTotalSalesAsync(int sellerId)
    {
        return await _context.Orders
            .Where(o => o.SellerId == sellerId &&
                       o.Status != Domain.Enums.OrderStatus.Cancelled)
            .SumAsync(o => o.TotalPrice - o.DiscountApplied);
    }

    public async Task<IReadOnlyList<Seller>> GetTopSellersBySalesAsync(int count = 10)
    {
        var sellerSales = await _context.Orders
            .Where(o => o.SellerId.HasValue && o.Status != Domain.Enums.OrderStatus.Cancelled)
            .GroupBy(o => o.SellerId!.Value)
            .Select(g => new
            {
                SellerId = g.Key,
                TotalSales = g.Sum(o => o.TotalPrice - o.DiscountApplied)
            })
            .OrderByDescending(x => x.TotalSales)
            .Take(count)
            .ToListAsync();

        var sellerIds = sellerSales.Select(x => x.SellerId).ToList();

        return await _dbSet
            .Where(s => sellerIds.Contains(s.Id))
            .ToListAsync();
    }

    public async Task<bool> IsPhoneExistsAsync(string phone, int? excludeSellerId = null)
    {
        var query = _dbSet.Where(x => x.Phone == phone);

        if (excludeSellerId.HasValue)
            query = query.Where(x => x.Id != excludeSellerId.Value);

        return await query.AnyAsync();
    }
}
