using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Repositories.Users;
using ZiyoNur.Service.Services.Interfaces;

namespace ZiyoNur.Service.EventHandlers.Products;

public class ProductOutOfStockEventHandler : INotificationHandler<ProductOutOfStockEvent>
{
    private readonly INotificationService _notificationService;
    private readonly IAdminRepository _adminRepository;
    private readonly ILogger<ProductOutOfStockEventHandler> _logger;

    public ProductOutOfStockEventHandler(
        INotificationService notificationService,
        IAdminRepository adminRepository,
        ILogger<ProductOutOfStockEventHandler> logger)
    {
        _notificationService = notificationService;
        _adminRepository = adminRepository;
        _logger = logger;
    }

    public async Task Handle(ProductOutOfStockEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Get all active admins
            var admins = await _adminRepository.GetActiveAdminsAsync();

            // Notify each admin
            foreach (var admin in admins)
            {
                await _notificationService.CreateInAppNotificationAsync(
                    userId: admin.Id,
                    userType: "admin",
                    title: "Mahsulot tugadi ⚠️",
                    message: $"{notification.ProductName} mahsuloti zaxirasi tugadi. Yangi zaxira qo'shing.",
                    notificationType: "product_out_of_stock",
                    actionUrl: $"/admin/products/{notification.ProductId}"
                );

                // Send push notification to admin
                if (!string.IsNullOrEmpty(admin.FcmToken))
                {
                    await _notificationService.SendPushNotificationAsync(
                        fcmToken: admin.FcmToken,
                        title: "Mahsulot tugadi",
                        message: $"{notification.ProductName} zaxirasi tugadi",
                        data: new Dictionary<string, string>
                        {
                            ["productId"] = notification.ProductId.ToString(),
                            ["action"] = "product_out_of_stock"
                        }
                    );
                }
            }

            _logger.LogInformation("Product out of stock event handled for product {ProductId}: {ProductName}",
                notification.ProductId, notification.ProductName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling ProductOutOfStockEvent for product {ProductId}", notification.ProductId);
        }
    }
}
