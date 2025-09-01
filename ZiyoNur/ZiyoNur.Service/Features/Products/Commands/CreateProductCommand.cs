using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;

namespace ZiyoNur.Service.Features.Products.Commands;

public class CreateProductCommand : BaseRequest<BaseResponse<ProductDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Count { get; set; }
    public int CategoryId { get; set; }
    public string? QrCode { get; set; }
    public string? SearchKeywords { get; set; }
    public int CreatedById { get; set; } // Admin/Seller who created

    public CreateProductCommand(CreateProductRequest request, int createdById)
    {
        Name = request.Name;
        Description = request.Description;
        Price = request.Price;
        Count = request.Count;
        CategoryId = request.CategoryId;
        QrCode = request.QrCode;
        SearchKeywords = request.SearchKeywords;
        CreatedById = createdById;
    }
}