using Microsoft.EntityFrameworkCore;
using ZiyoNur.Data.Common;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Enums;
using ZiyoNur.Domain.Repositories.Products;

namespace ZiyoNur.Data.Configurations.Repositories.Products;

public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public ProductRepository(MarketplaceDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByQrCodeAsync(string qrCode)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(x => x.QrCode == qrCode);
    }

    public async Task<Product?> GetProductWithCategoryAsync(int productId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .FirstOrDefaultAsync(x => x.Id == productId);
    }

    public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(int categoryId, bool includeSubcategories = false)
    {
        if (!includeSubcategories)
        {
            return await _dbSet
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId && p.Status == ProductStatus.Active)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        // Get subcategory IDs
        var subcategoryIds = await _context.Categories
            .Where(c => c.ParentId == categoryId)
            .Select(c => c.Id)
            .ToListAsync();

        subcategoryIds.Add(categoryId); // Include parent category

        return await _dbSet
            .Include(p => p.Category)
            .Where(p => subcategoryIds.Contains(p.CategoryId) && p.Status == ProductStatus.Active)
            .OrderBy(p => p.Category.Name)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm)
    {
        var normalizedSearch = searchTerm.ToLower().Trim();

        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.Status == ProductStatus.Active &&
                       (p.Name.ToLower().Contains(normalizedSearch) ||
                        p.Description != null && p.Description.ToLower().Contains(normalizedSearch) ||
                        p.SearchKeywords != null && p.SearchKeywords.ToLower().Contains(normalizedSearch) ||
                        p.Category.Name.ToLower().Contains(normalizedSearch)))
            .OrderBy(p => p.Name.ToLower().IndexOf(normalizedSearch)) // Exact matches first
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetAvailableProductsAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.Status == ProductStatus.Active && p.Count > 0)
            .OrderBy(p => p.Category.Name)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetOutOfStockProductsAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => (p.Status == ProductStatus.Active || p.Status == ProductStatus.OutOfStock) && p.Count <= 0)
            .OrderBy(p => p.Category.Name)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetLowStockProductsAsync(int threshold = 10)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.Status == ProductStatus.Active && p.Count <= threshold && p.Count > 0)
            .OrderBy(p => p.Count)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetTopSellingProductsAsync(int count = 10)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.Status == ProductStatus.Active)
            .OrderByDescending(p => p.SoldCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Product>> GetMostLikedProductsAsync(int count = 10)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.Status == ProductStatus.Active)
            .OrderByDescending(p => p.LikeCount)
            .Take(count)
            .ToListAsync();
    }

    public async Task<bool> IsQrCodeExistsAsync(string qrCode, int? excludeProductId = null)
    {
        var query = _dbSet.Where(x => x.QrCode == qrCode);

        if (excludeProductId.HasValue)
            query = query.Where(x => x.Id != excludeProductId.Value);

        return await query.AnyAsync();
    }

    public async Task UpdateStockAsync(int productId, int newStock)
    {
        var product = await _dbSet.FindAsync(productId);
        if (product != null)
        {
            product.Count = newStock;
            product.Status = newStock > 0 ? ProductStatus.Active : ProductStatus.OutOfStock;
            _context.Entry(product).State = EntityState.Modified;
        }
    }

    public async Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedProductsAsync(
        string? searchTerm = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        int pageIndex = 0,
        int pageSize = 20)
    {
        IQueryable<Product> query = _dbSet.Include(p => p.Category);

        // Apply filters
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearch = searchTerm.ToLower().Trim();
            query = query.Where(p => p.Name.ToLower().Contains(normalizedSearch) ||
                                   (p.Description != null && p.Description.ToLower().Contains(normalizedSearch)) ||
                                   (p.SearchKeywords != null && p.SearchKeywords.ToLower().Contains(normalizedSearch)));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId.Value);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        if (inStock.HasValue)
        {
            if (inStock.Value)
                query = query.Where(p => p.Count > 0);
            else
                query = query.Where(p => p.Count <= 0);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(p => p.Name)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
