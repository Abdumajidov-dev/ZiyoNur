using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Enums;
using ZiyoNur.Domain.Repositories.Orders;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Common;
using ZiyoNur.Service.DTOs.Orders;
using ZiyoNur.Service.Features.Orders.Commands;

namespace ZiyoNur.Service.Features.Orders.Handlers;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, BaseResponse<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly ICashbackTransactionRepository _cashbackRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICustomerRepository customerRepository,
        ICashbackTransactionRepository cashbackRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _customerRepository = customerRepository;
        _cashbackRepository = cashbackRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaseResponse<OrderDto>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Validate customer
            var customer = await _customerRepository.GetByIdAsync(request.CustomerId);
            if (customer == null || !customer.IsActive)
            {
                return BaseResponse<OrderDto>.Failure("Mijoz topilmadi");
            }

            // Validate cashback usage
            if (request.CashbackToUse > 0)
            {
                var availableCashback = await _cashbackRepository.GetCustomerAvailableCashbackAmountAsync(request.CustomerId);
                if (request.CashbackToUse > availableCashback)
                {
                    return BaseResponse<OrderDto>.Failure($"Mavjud cashback: {availableCashback:N0} so'm. Siz {request.CashbackToUse:N0} so'm ishlatmoqchi edingiz");
                }
            }

            // Create order
            var order = new Order
            {
                CustomerId = request.CustomerId,
                SellerId = request.SellerId,
                PaymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod, true),
                DeliveryType = Enum.Parse<DeliveryType>(request.DeliveryType, true),
                OrderSource = request.OrderSource,
                Notes = request.Notes,
                Status = OrderStatus.Pending,
                PaymentStatus = PaymentStatus.Pending
            };

            // Add order items and calculate total
            decimal totalPrice = 0;
            foreach (var itemRequest in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemRequest.ProductId);
                if (product == null)
                {
                    return BaseResponse<OrderDto>.Failure($"Mahsulot topilmadi: {itemRequest.ProductId}");
                }

                if (!product.CanOrder(itemRequest.Quantity))
                {
                    return BaseResponse<OrderDto>.Failure($"Yetarli zaxira yo'q: {product.Name}. Mavjud: {product.Count}");
                }

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemRequest.Quantity,
                    UnitPrice = product.Price,
                    TotalPrice = product.Price * itemRequest.Quantity
                };

                order.OrderItems.Add(orderItem);
                totalPrice += orderItem.TotalPrice;

                // Update product stock
                product.UpdateStock(-itemRequest.Quantity, "Order created");
                _productRepository.Update(product);
            }

            order.TotalPrice = totalPrice;

            // Apply cashback if requested
            if (request.CashbackToUse > 0)
            {
                var cashbackTransactions = await _cashbackRepository.GetAvailableCashbackAsync(request.CustomerId);
                var remainingCashback = request.CashbackToUse;

                // Use FIFO approach for cashback
                foreach (var cashback in cashbackTransactions)
                {
                    if (remainingCashback <= 0) break;

                    var amountToUse = Math.Min(cashback.Amount, remainingCashback);
                    cashback.MarkAsUsed();

                    // Create used cashback transaction
                    var usedTransaction = new CashbackTransaction
                    {
                        CustomerId = request.CustomerId,
                        OrderId = order.Id,
                        Amount = -amountToUse,
                        Type = CashbackTransactionType.Used
                    };

                    order.CashbackTransactions.Add(usedTransaction);
                    remainingCashback -= amountToUse;
                }

                order.DiscountApplied = request.CashbackToUse;
            }

            // Save order
            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Load order with details for response
            var orderWithDetails = await _orderRepository.GetOrderWithDetailsAsync(order.Id);
            var orderDto = _mapper.Map<OrderDto>(orderWithDetails);

            await _unitOfWork.CommitTransactionAsync();

            _logger.LogInformation("Order created successfully: {OrderId} for customer: {CustomerId}", order.Id, request.CustomerId);
            return BaseResponse<OrderDto>.Success(orderDto, "Buyurtma muvaffaqiyatli yaratildi");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError(ex, "Error creating order for customer: {CustomerId}", request.CustomerId);
            return BaseResponse<OrderDto>.Failure("Buyurtma yaratishda xatolik yuz berdi");
        }
    }
}