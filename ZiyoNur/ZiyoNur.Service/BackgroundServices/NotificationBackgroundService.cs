using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Repositories.Notifications;
using ZiyoNur.Service.Services.Interfaces;

namespace ZiyoNur.Service.BackgroundServices;

public class NotificationBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationBackgroundService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1); // Check every minute

    public NotificationBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<NotificationBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessPendingNotifications();
                await Task.Delay(_interval, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing background notifications");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes on error
            }
        }
    }

    private async Task ProcessPendingNotifications()
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

        try
        {
            // Get unsent notifications
            var pendingNotifications = await notificationRepository.GetUnsentNotificationsAsync();

            if (!pendingNotifications.Any()) return;

            _logger.LogInformation("Processing {Count} pending notifications", pendingNotifications.Count);

            foreach (var notification in pendingNotifications)
            {
                try
                {
                    // Get user's FCM token based on user type
                    string? fcmToken = await GetUserFcmToken(scope.ServiceProvider, notification.UserId, notification.UserType);

                    if (!string.IsNullOrEmpty(fcmToken))
                    {
                        var success = await notificationService.SendPushNotificationAsync(
                            fcmToken: fcmToken,
                            title: notification.Title,
                            message: notification.Message,
                            data: new Dictionary<string, string>
                            {
                                ["notificationId"] = notification.Id.ToString(),
                                ["type"] = notification.NotificationType.Name,
                                ["actionUrl"] = notification.ActionUrl ?? ""
                            }
                        );

                        if (success)
                        {
                            notification.MarkAsSent();
                            notificationRepository.Update(notification);
                        }
                    }
                    else
                    {
                        // Mark as sent even if no FCM token (in-app notification only)
                        notification.MarkAsSent();
                        notificationRepository.Update(notification);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error sending notification {NotificationId}", notification.Id);
                }
            }

            // Save all changes
            await scope.ServiceProvider.GetRequiredService<Domain.Common.IUnitOfWork>().SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ProcessPendingNotifications");
        }
    }

    private async Task<string?> GetUserFcmToken(IServiceProvider serviceProvider, int userId, string userType)
    {
        try
        {
            return userType.ToLower() switch
            {
                "customer" => (await serviceProvider.GetRequiredService<Domain.Repositories.Users.ICustomerRepository>()
                    .GetByIdAsync(userId))?.FcmToken,
                "seller" => (await serviceProvider.GetRequiredService<Domain.Repositories.Users.ISellerRepository>()
                    .GetByIdAsync(userId))?.FcmToken,
                "admin" => (await serviceProvider.GetRequiredService<Domain.Repositories.Users.IAdminRepository>()
                    .GetByIdAsync(userId))?.FcmToken,
                _ => null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting FCM token for user {UserId} ({UserType})", userId, userType);
            return null;
        }
    }
}
