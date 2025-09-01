using ZiyoNur.Domain.Entities.Products;

namespace ZiyoNur.Domain.Repositories.Products;

public interface ICategoryRepository : IBaseRepository<Category>
{
    Task<IReadOnlyList<Category>> GetRootCategoriesAsync();
    Task<IReadOnlyList<Category>> GetSubCategoriesAsync(int parentId);
    Task<Category?> GetCategoryWithProductsAsync(int categoryId);
    Task<Category?> GetCategoryWithSubCategoriesAsync(int categoryId);
    Task<IReadOnlyList<Category>> GetActiveCategoriesAsync();
    Task<bool> HasProductsAsync(int categoryId);
    Task<bool> HasSubCategoriesAsync(int categoryId);
}
