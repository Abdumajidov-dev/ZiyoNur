using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Products;
using ZiyoNur.Service.Features.Products.Commands;

namespace ZiyoNur.Service.Features.Products.Handlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, BaseResponse<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<ProductDto>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Get existing product
            var product = await _productRepository.GetProductWithCategoryAsync(request.Id);
            if (product == null)
            {
                return BaseResponse<ProductDto>.Failure("Mahsulot topilmadi");
            }

            // Validate category
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null || !category.IsActive)
            {
                return BaseResponse<ProductDto>.Failure("Kategoriya topilmadi");
            }

            // Check QR code uniqueness (exclude current product)
            if (!string.IsNullOrEmpty(request.QrCode) &&
                await _productRepository.IsQrCodeExistsAsync(request.QrCode, request.Id))
            {
                return BaseResponse<ProductDto>.Failure("Bu QR kod boshqa mahsulot tomonidan ishlatilmoqda");
            }

            // Update product
            var oldPrice = product.Price;
            product.Name = request.Name;
            product.Description = request.Description;
            product.CategoryId = request.CategoryId;
            product.QrCode = request.QrCode;
            product.SearchKeywords = request.SearchKeywords;
            product.UpdatedById = request.UpdatedById;
            product.UpdatedByType = "admin";

            // Update price using domain method (triggers price change event)
            if (oldPrice != request.Price)
            {
                product.SetPrice(request.Price);
            }

            // Update stock using domain method (triggers stock change event)
            if (product.Count != request.Count)
            {
                var stockDifference = request.Count - product.Count;
                product.UpdateStock(stockDifference, "Manual stock adjustment by admin");
            }

            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var productDto = _mapper.Map<ProductDto>(product);

            _logger.LogInformation("Product updated successfully: {ProductId} by admin {AdminId}", product.Id, request.UpdatedById);
            return BaseResponse<ProductDto>.Success(productDto, "Mahsulot yangilandi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating product {ProductId}", request.Id);
            return BaseResponse<ProductDto>.Failure("Mahsulotni yangilashda xatolik yuz berdi");
        }
    }
}