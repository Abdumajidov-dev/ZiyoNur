using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;
using ZiyoNur.Service.Features.Products.Commands;

namespace ZiyoNur.Service.Features.Products.Handlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, BaseResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate category
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null || !category.IsActive)
            {
                return BaseResponse<ProductDto>.Failure("Kategoriya topilmadi");
            }

            // Check QR code uniqueness if provided
            if (!string.IsNullOrEmpty(request.QrCode) &&
                await _productRepository.IsQrCodeExistsAsync(request.QrCode))
            {
                return BaseResponse<ProductDto>.Failure("Bu QR kod allaqachon ishlatilmoqda");
            }

            // Create new product
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Count = request.Count,
                CategoryId = request.CategoryId,
                QrCode = request.QrCode,
                SearchKeywords = request.SearchKeywords,
                Status = Domain.Enums.ProductStatus.Active,
                CreatedById = request.CreatedById,
                CreatedByType = "admin"
            };

            await _productRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Load product with category for DTO mapping
            var productWithCategory = await _productRepository.GetProductWithCategoryAsync(product.Id);
            var productDto = _mapper.Map<ProductDto>(productWithCategory);

            _logger.LogInformation("Product created successfully: {ProductId} by admin {AdminId}", product.Id, request.CreatedById);
            return BaseResponse<ProductDto>.Success(productDto, "Mahsulot yaratildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating product");
            return BaseResponse<ProductDto>.Failure("Mahsulot yaratishda xatolik yuz berdi");
        }
    }
}