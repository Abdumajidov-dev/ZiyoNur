using ZiyoNur.Domain.Entities.Users;

namespace ZiyoNur.Domain.Repositories.Users;

public interface IAdminRepository : IBaseRepository<Admin>
{
    Task<Admin?> GetByUsernameAsync(string username);
    Task<Admin?> GetByPhoneAsync(string phone);
    Task<IReadOnlyList<Admin>> GetActiveAdminsAsync();
    Task<bool> IsUsernameExistsAsync(string username, int? excludeAdminId = null);
    Task<bool> IsPhoneExistsAsync(string phone, int? excludeAdminId = null);
}
