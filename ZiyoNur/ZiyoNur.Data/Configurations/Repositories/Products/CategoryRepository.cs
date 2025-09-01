using Microsoft.EntityFrameworkCore;
using ZiyoNur.Data.Common;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Enums;
using ZiyoNur.Domain.Repositories.Products;

namespace ZiyoNur.Data.Configurations.Repositories.Products;

public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
    public CategoryRepository(MarketplaceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.ParentId == null && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Category>> GetSubCategoriesAsync(int parentId)
    {
        return await _dbSet
            .Where(c => c.ParentId == parentId && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryWithProductsAsync(int categoryId)
    {
        return await _dbSet
            .Include(c => c.Products.Where(p => p.Status == ProductStatus.Active))
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task<Category?> GetCategoryWithSubCategoriesAsync(int categoryId)
    {
        return await _dbSet
            .Include(c => c.SubCategories.Where(sc => sc.IsActive))
            .FirstOrDefaultAsync(c => c.Id == categoryId);
    }

    public async Task<IReadOnlyList<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.ParentId ?? 0) // Root categories first
            .ThenBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Products
            .AnyAsync(p => p.CategoryId == categoryId);
    }

    public async Task<bool> HasSubCategoriesAsync(int categoryId)
    {
        return await _dbSet
            .AnyAsync(c => c.ParentId == categoryId);
    }
}
