using System.ComponentModel.DataAnnotations;
using ZiyoNur.Service.DTOs.Common;

namespace ZiyoNur.Service.DTOs.Products;

public class ProductDto : AuditableDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Count { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? QrCode { get; set; }
    public string? ImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusText { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public int LikeCount { get; set; }
    public int SoldCount { get; set; }
    public decimal AverageRating { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsLikedByUser { get; set; } // For current user
    public bool IsInUserCart { get; set; } // For current user
}

public class ProductListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public string? CategoryName { get; set; }
    public bool IsAvailable { get; set; }
    public int LikeCount { get; set; }
    public bool IsLikedByUser { get; set; }
}

public class CreateProductRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Count { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public string? QrCode { get; set; }
    public string? SearchKeywords { get; set; }
}

public class UpdateProductRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Count { get; set; }

    [Required]
    public int CategoryId { get; set; }

    public string? QrCode { get; set; }
    public string? SearchKeywords { get; set; }
}

public class ProductSearchRequest
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public string? SortBy { get; set; } // name, price, created, popularity
    public bool SortDescending { get; set; }
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 20;
}
