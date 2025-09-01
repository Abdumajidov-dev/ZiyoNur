using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Services.Interfaces;

namespace ZiyoNur.Service.EventHandlers.Orders;

public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(
        INotificationService notificationService,
        ICustomerRepository customerRepository,
        ILogger<OrderCreatedEventHandler> logger)
    {
        _notificationService = notificationService;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Get customer details
            var customer = await _customerRepository.GetByIdAsync(notification.CustomerId);
            if (customer == null) return;

            // Send notification to customer
            await _notificationService.CreateInAppNotificationAsync(
                userId: notification.CustomerId,
                userType: "customer",
                title: "Buyurtma yaratildi",
                message: $"#{notification.OrderId} raqamli buyurtmangiz muvaffaqiyatli yaratildi. Jami summa: {notification.TotalAmount:N0} so'm",
                notificationType: "order_created",
                actionUrl: $"/orders/{notification.OrderId}"
            );

            // Send push notification if customer has FCM token
            if (!string.IsNullOrEmpty(customer.FcmToken))
            {
                await _notificationService.SendPushNotificationAsync(
                    fcmToken: customer.FcmToken,
                    title: "Buyurtma yaratildi",
                    message: $"#{notification.OrderId} buyurtmangiz yaratildi",
                    data: new Dictionary<string, string>
                    {
                        ["orderId"] = notification.OrderId.ToString(),
                        ["action"] = "order_created"
                    }
                );
            }

            // TODO: Send notification to admin/sellers about new order
            // await NotifyAdminsAboutNewOrder(notification);

            _logger.LogInformation("Order created event handled for order {OrderId}", notification.OrderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling OrderCreatedEvent for order {OrderId}", notification.OrderId);
        }
    }
}