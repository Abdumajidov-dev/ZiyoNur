using FluentValidation;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.DTOs.Orders;
using ZiyoNur.Service.Features.Orders.Commands;

namespace ZiyoNur.Service.Validators.Orders;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandValidator(
        ICustomerRepository customerRepository,
        IProductRepository productRepository)
    {
        _customerRepository = customerRepository;
        _productRepository = productRepository;

        RuleFor(x => x.CustomerId)
            .MustAsync(CustomerExists).WithMessage("Mijoz topilmadi");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Buyurtmada kamida bitta mahsulot bo'lishi kerak");

        RuleForEach(x => x.Items).SetValidator(new CreateOrderItemValidator(_productRepository));

        RuleFor(x => x.PaymentMethod)
            .Must(x => new[] { "payme", "uzcard", "click", "humo", "cash", "cashback", "mixed" }
                .Contains(x.ToLower()))
            .WithMessage("To'lov usuli noto'g'ri");

        RuleFor(x => x.DeliveryType)
            .Must(x => new[] { "pickup", "delivery" }.Contains(x.ToLower()))
            .WithMessage("Yetkazib berish turi: pickup yoki delivery");

        RuleFor(x => x.DeliveryAddress)
            .NotEmpty().WithMessage("Yetkazib berish manzili majburiy")
            .When(x => x.DeliveryType.ToLower() == "delivery");

        RuleFor(x => x.CashbackToUse)
            .GreaterThanOrEqualTo(0).WithMessage("Cashback manfiy bo'lmasligi kerak");
    }

    private async Task<bool> CustomerExists(int customerId, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        return customer != null && customer.IsActive;
    }
}

public class CreateOrderItemValidator : AbstractValidator<CreateOrderItemRequest>
{
    private readonly IProductRepository _productRepository;

    public CreateOrderItemValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;

        RuleFor(x => x.ProductId)
            .MustAsync(ProductExists).WithMessage("Mahsulot topilmadi");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Miqdor 0 dan katta bo'lishi kerak");
    }

    private async Task<bool> ProductExists(int productId, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        return product != null && product.IsAvailable;
    }
}