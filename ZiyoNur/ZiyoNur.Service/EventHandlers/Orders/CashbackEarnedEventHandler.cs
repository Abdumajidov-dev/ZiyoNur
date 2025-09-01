using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Services.Interfaces;

namespace ZiyoNur.Service.EventHandlers.Orders;

public class CashbackEarnedEventHandler : INotificationHandler<CashbackEarnedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ICustomerRepository _customerRepository;
    private readonly ILogger<CashbackEarnedEventHandler> _logger;

    public CashbackEarnedEventHandler(
        INotificationService notificationService,
        ICustomerRepository customerRepository,
        ILogger<CashbackEarnedEventHandler> logger)
    {
        _notificationService = notificationService;
        _customerRepository = customerRepository;
        _logger = logger;
    }

    public async Task Handle(CashbackEarnedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Get customer details
            var customer = await _customerRepository.GetByIdAsync(notification.CustomerId);
            if (customer == null) return;

            // Send cashback notification
            await _notificationService.CreateInAppNotificationAsync(
                userId: notification.CustomerId,
                userType: "customer",
                title: "Cashback yig'ildingiz! 🎉",
                message: $"{notification.Amount:N0} so'm cashback hisobingizga qo'shildi. 30 kun davomida ishlatishingiz mumkin.",
                notificationType: "cashback_earned",
                actionUrl: "/profile/cashback"
            );

            // Send push notification
            if (!string.IsNullOrEmpty(customer.FcmToken))
            {
                await _notificationService.SendPushNotificationAsync(
                    fcmToken: customer.FcmToken,
                    title: "Cashback yig'ildingiz!",
                    message: $"{notification.Amount:N0} so'm cashback qo'shildi",
                    data: new Dictionary<string, string>
                    {
                        ["cashbackAmount"] = notification.Amount.ToString(),
                        ["orderId"] = notification.OrderId.ToString(),
                        ["action"] = "cashback_earned"
                    }
                );
            }

            _logger.LogInformation("Cashback earned event handled for customer {CustomerId}, amount: {Amount}",
                notification.CustomerId, notification.Amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling CashbackEarnedEvent for customer {CustomerId}", notification.CustomerId);
        }
    }
}