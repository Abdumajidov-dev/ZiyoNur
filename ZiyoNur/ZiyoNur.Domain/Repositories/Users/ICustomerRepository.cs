using ZiyoNur.Domain.Entities.Users;

namespace ZiyoNur.Domain.Repositories.Users;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer?> GetByPhoneAsync(string phone);
    Task<Customer?> GetByEmailAsync(string email);
    Task<Customer?> GetCustomerWithOrdersAsync(int customerId);
    Task<Customer?> GetCustomerWithCartAsync(int customerId);
    Task<IReadOnlyList<Customer>> GetActiveCustomersAsync();
    Task<IReadOnlyList<Customer>> GetCustomersByRegistrationDateAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetCustomerTotalSpentAsync(int customerId);
    Task<int> GetCustomerOrderCountAsync(int customerId);
    Task<IReadOnlyList<Customer>> GetTopCustomersBySpendingAsync(int count = 10);
    Task<bool> IsPhoneExistsAsync(string phone, int? excludeCustomerId = null);
    Task<bool> IsEmailExistsAsync(string email, int? excludeCustomerId = null);
}
