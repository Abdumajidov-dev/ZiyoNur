using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.Features.Cart.Commands;

namespace ZiyoNur.Service.Features.Cart.Handlers;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, BaseResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AddToCartCommandHandler> _logger;

    public AddToCartCommandHandler(
        ICustomerRepository customerRepository,
        IProductRepository productRepository,
        IUnitOfWork unitOfWork,
        ILogger<AddToCartCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<BaseResponse> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Validate customer
            var customer = await _customerRepository.GetCustomerWithCartAsync(request.CustomerId);
            if (customer == null || !customer.IsActive)
            {
                return BaseResponse.Failure("Mijoz topilmadi");
            }

            // Validate product
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null || !product.IsAvailable)
            {
                return BaseResponse.Failure("Mahsulot mavjud emas");
            }

            // Check if enough stock
            var existingCartItem = customer.CartItems.FirstOrDefault(c => c.ProductId == request.ProductId);
            var totalQuantity = request.Quantity + (existingCartItem?.Quantity ?? 0);

            if (!product.CanOrder(totalQuantity))
            {
                return BaseResponse.Failure($"Yetarli zaxira yo'q. Mavjud: {product.Count} dona");
            }

            // Add to cart
            customer.AddToCart(request.ProductId, request.Quantity);

            _customerRepository.Update(customer);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Product {ProductId} added to cart for customer {CustomerId}", request.ProductId, request.CustomerId);
            return BaseResponse.Success("Mahsulot savatga qo'shildi");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding product {ProductId} to cart for customer {CustomerId}", request.ProductId, request.CustomerId);
            return BaseResponse.Failure("Savatga qo'shishda xatolik yuz berdi");
        }
    }
}