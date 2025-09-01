using ZiyoNur.Domain.Entities.Users;

namespace ZiyoNur.Domain.Repositories.Users;

public interface ISellerRepository : IBaseRepository<Seller>
{
    Task<Seller?> GetByPhoneAsync(string phone);
    Task<Seller?> GetSellerWithOrdersAsync(int sellerId);
    Task<IReadOnlyList<Seller>> GetActiveSellerAsync();
    Task<decimal> GetSellerTotalSalesAsync(int sellerId);
    Task<IReadOnlyList<Seller>> GetTopSellersBySalesAsync(int count = 10);
    Task<bool> IsPhoneExistsAsync(string phone, int? excludeSellerId = null);
}
