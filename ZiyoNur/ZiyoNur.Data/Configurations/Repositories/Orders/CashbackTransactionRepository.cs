using Microsoft.EntityFrameworkCore;
using ZiyoNur.Data.Common;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Enums;
using ZiyoNur.Domain.Repositories.Orders;

namespace ZiyoNur.Data.Configurations.Repositories.Orders;

public class CashbackTransactionRepository : BaseRepository<CashbackTransaction>, ICashbackTransactionRepository
{
    public CashbackTransactionRepository(MarketplaceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<CashbackTransaction>> GetCustomerCashbackAsync(int customerId)
    {
        return await _dbSet
            .Include(c => c.Order)
            .Where(c => c.CustomerId == customerId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CashbackTransaction>> GetAvailableCashbackAsync(int customerId)
    {
        return await _dbSet
            .Include(c => c.Order)
            .Where(c => c.CustomerId == customerId &&
                       c.Type == CashbackTransactionType.Earned &&
                       !c.IsUsed &&
                       (c.ExpiryDate == null || c.ExpiryDate > DateTime.UtcNow))
            .OrderBy(c => c.ExpiryDate ?? DateTime.MaxValue) // FIFO - oldest first
            .ToListAsync();
    }

    public async Task<IReadOnlyList<CashbackTransaction>> GetExpiredCashbackAsync(int customerId)
    {
        return await _dbSet
            .Include(c => c.Order)
            .Where(c => c.CustomerId == customerId &&
                       c.Type == CashbackTransactionType.Earned &&
                       !c.IsUsed &&
                       c.ExpiryDate.HasValue &&
                       c.ExpiryDate < DateTime.UtcNow)
            .OrderByDescending(c => c.ExpiryDate)
            .ToListAsync();
    }

    public async Task<decimal> GetCustomerAvailableCashbackAmountAsync(int customerId)
    {
        return await _dbSet
            .Where(c => c.CustomerId == customerId &&
                       c.Type == CashbackTransactionType.Earned &&
                       !c.IsUsed &&
                       (c.ExpiryDate == null || c.ExpiryDate > DateTime.UtcNow))
            .SumAsync(c => c.Amount);
    }

    public async Task<IReadOnlyList<CashbackTransaction>> GetExpiringCashbackAsync(int days = 7)
    {
        var expiryThreshold = DateTime.UtcNow.AddDays(days);

        return await _dbSet
            .Include(c => c.Customer)
            .Where(c => c.Type == CashbackTransactionType.Earned &&
                       !c.IsUsed &&
                       c.ExpiryDate.HasValue &&
                       c.ExpiryDate <= expiryThreshold &&
                       c.ExpiryDate > DateTime.UtcNow)
            .OrderBy(c => c.ExpiryDate)
            .ToListAsync();
    }
}