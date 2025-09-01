using Microsoft.EntityFrameworkCore;
using ZiyoNur.Data.Common;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Repositories.Users;

namespace ZiyoNur.Data.Configurations.Repositories.Users;

public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    public CustomerRepository(MarketplaceDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByPhoneAsync(string phone)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Phone == phone);
    }

    public async Task<Customer?> GetByEmailAsync(string email)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<Customer?> GetCustomerWithOrdersAsync(int customerId)
    {
        return await _dbSet
            .Include(x => x.Orders)
                .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(x => x.Id == customerId);
    }

    public async Task<Customer?> GetCustomerWithCartAsync(int customerId)
    {
        return await _dbSet
            .Include(x => x.CartItems)
                .ThenInclude(c => c.Product)
                    .ThenInclude(p => p.Category)
            .FirstOrDefaultAsync(x => x.Id == customerId);
    }

    public async Task<IReadOnlyList<Customer>> GetActiveCustomersAsync()
    {
        return await _dbSet
            .Where(x => x.IsActive)
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Customer>> GetCustomersByRegistrationDateAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
            .OrderBy(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<decimal> GetCustomerTotalSpentAsync(int customerId)
    {
        return await _context.Orders
            .Where(o => o.CustomerId == customerId &&
                       o.Status != Domain.Enums.OrderStatus.Cancelled)
            .SumAsync(o => o.TotalPrice - o.DiscountApplied);
    }

    public async Task<int> GetCustomerOrderCountAsync(int customerId)
    {
        return await _context.Orders
            .CountAsync(o => o.CustomerId == customerId &&
                           o.Status != Domain.Enums.OrderStatus.Cancelled);
    }

    public async Task<IReadOnlyList<Customer>> GetTopCustomersBySpendingAsync(int count = 10)
    {
        var customerSpending = await _context.Orders
            .Where(o => o.Status != Domain.Enums.OrderStatus.Cancelled)
            .GroupBy(o => o.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                TotalSpent = g.Sum(o => o.TotalPrice - o.DiscountApplied)
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(count)
            .ToListAsync();

        var customerIds = customerSpending.Select(x => x.CustomerId).ToList();

        return await _dbSet
            .Where(c => customerIds.Contains(c.Id))
            .ToListAsync();
    }

    public async Task<bool> IsPhoneExistsAsync(string phone, int? excludeCustomerId = null)
    {
        var query = _dbSet.Where(x => x.Phone == phone);

        if (excludeCustomerId.HasValue)
            query = query.Where(x => x.Id != excludeCustomerId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeCustomerId = null)
    {
        var query = _dbSet.Where(x => x.Email == email);

        if (excludeCustomerId.HasValue)
            query = query.Where(x => x.Id != excludeCustomerId.Value);

        return await query.AnyAsync();
    }
}
