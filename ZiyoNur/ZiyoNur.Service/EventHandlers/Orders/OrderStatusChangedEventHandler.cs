using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Services.Interfaces;

namespace ZiyoNur.Service.EventHandlers.Orders;

public class OrderStatusChangedEventHandler : INotificationHandler<OrderStatusChangedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<OrderStatusChangedEventHandler> _logger;

    public OrderStatusChangedEventHandler(
        INotificationService notificationService,
        ICustomerRepository customerRepository,
        ILogger<OrderStatusChangedEventHandler> logger)
    {
        _notificationService = notificationService;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task Handle(OrderStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Get order details (you might need to inject order repository)
            // For now, we'll use the event data

            var statusMessage = GetStatusMessage(notification.NewStatus);
            var title = "Buyurtma holati o'zgardi";

            // Send in-app notification
            // Note: We need to get customer ID from order
            // This is simplified - in real implementation, you'd get customer ID from order repository

            await _notificationService.CreateInAppNotificationAsync(
                userId: 0, // TODO: Get customer ID from order
                userType: "customer",
                title: title,
                message: $"#{notification.OrderId} buyurtmangiz holati: {statusMessage}",
                notificationType: "order_status_changed",
                actionUrl: $"/orders/{notification.OrderId}"
            );

            _logger.LogInformation("Order status changed event handled for order {OrderId}: {OldStatus} -> {NewStatus}",
                notification.OrderId, notification.OldStatus, notification.NewStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling OrderStatusChangedEvent for order {OrderId}", notification.OrderId);
        }
    }

    private static string GetStatusMessage(Domain.Enums.OrderStatus status)
    {
        return status switch
        {
            Domain.Enums.OrderStatus.Confirmed => "Tasdiqlandi",
            Domain.Enums.OrderStatus.Preparing => "Tayyorlanmoqda",
            Domain.Enums.OrderStatus.ReadyForPickup => "Olib ketishga tayyor",
            Domain.Enums.OrderStatus.Shipped => "Jo'natildi",
            Domain.Enums.OrderStatus.Delivered => "Yetkazildi",
            Domain.Enums.OrderStatus.Cancelled => "Bekor qilindi",
            _ => status.ToString()
        };
    }
}
