using Microsoft.EntityFrameworkCore;
using ZiyoNur.Data.Common;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Repositories.Users;

namespace ZiyoNur.Data.Configurations.Repositories.Users;

public class AdminRepository : BaseRepository<Admin>, IAdminRepository
{
    public AdminRepository(MarketplaceDbContext context) : base(context)
    {
    }

    public async Task<Admin?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Username == username);
    }

    public async Task<Admin?> GetByPhoneAsync(string phone)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.Phone == phone);
    }

    public async Task<IReadOnlyList<Admin>> GetActiveAdminsAsync()
    {
        return await _dbSet
            .Where(x => x.IsActive)
            .OrderBy(x => x.FirstName)
            .ThenBy(x => x.LastName)
            .ToListAsync();
    }

    public async Task<bool> IsUsernameExistsAsync(string username, int? excludeAdminId = null)
    {
        var query = _dbSet.Where(x => x.Username == username);

        if (excludeAdminId.HasValue)
            query = query.Where(x => x.Id != excludeAdminId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> IsPhoneExistsAsync(string phone, int? excludeAdminId = null)
    {
        var query = _dbSet.Where(x => x.Phone == phone);

        if (excludeAdminId.HasValue)
            query = query.Where(x => x.Id != excludeAdminId.Value);

        return await query.AnyAsync();
    }
}
