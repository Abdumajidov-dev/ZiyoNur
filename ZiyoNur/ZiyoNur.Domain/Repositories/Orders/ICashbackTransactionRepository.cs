using ZiyoNur.Domain.Entities.Orders;

namespace ZiyoNur.Domain.Repositories.Orders;

public interface ICashbackTransactionRepository : IBaseRepository<CashbackTransaction>
{
    Task<IReadOnlyList<CashbackTransaction>> GetCustomerCashbackAsync(int customerId);
    Task<IReadOnlyList<CashbackTransaction>> GetAvailableCashbackAsync(int customerId);
    Task<IReadOnlyList<CashbackTransaction>> GetExpiredCashbackAsync(int customerId);
    Task<decimal> GetCustomerAvailableCashbackAmountAsync(int customerId);
    Task<IReadOnlyList<CashbackTransaction>> GetExpiringCashbackAsync(int days = 7);
}
