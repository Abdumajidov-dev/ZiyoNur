using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;

namespace ZiyoNur.Service.Features.Products.Commands;

public class UpdateProductCommand : BaseRequest<BaseResponse<ProductDto>>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Count { get; set; }
    public int CategoryId { get; set; }
    public string? QrCode { get; set; }
    public string? SearchKeywords { get; set; }
    public int UpdatedById { get; set; }

    public UpdateProductCommand(int id, UpdateProductRequest request, int updatedById)
    {
        Id = id;
        Name = request.Name;
        Description = request.Description;
        Price = request.Price;
        Count = request.Count;
        CategoryId = request.CategoryId;
        QrCode = request.QrCode;
        SearchKeywords = request.SearchKeywords;
        UpdatedById = updatedById;
    }
}