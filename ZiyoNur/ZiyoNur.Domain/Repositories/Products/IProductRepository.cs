using ZiyoNur.Domain.Entities.Products;

namespace ZiyoNur.Domain.Repositories.Products;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<Product?> GetByQrCodeAsync(string qrCode);
    Task<Product?> GetProductWithCategoryAsync(int productId);
    Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(int categoryId, bool includeSubcategories = false);
    Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm);
    Task<IReadOnlyList<Product>> GetAvailableProductsAsync();
    Task<IReadOnlyList<Product>> GetOutOfStockProductsAsync();
    Task<IReadOnlyList<Product>> GetLowStockProductsAsync(int threshold = 10);
    Task<IReadOnlyList<Product>> GetTopSellingProductsAsync(int count = 10);
    Task<IReadOnlyList<Product>> GetMostLikedProductsAsync(int count = 10);
    Task<bool> IsQrCodeExistsAsync(string qrCode, int? excludeProductId = null);
    Task UpdateStockAsync(int productId, int newStock);
    Task<(IReadOnlyList<Product> Items, int TotalCount)> GetPagedProductsAsync(
        string? searchTerm = null,
        int? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? inStock = null,
        int pageIndex = 0,
        int pageSize = 20);
}
