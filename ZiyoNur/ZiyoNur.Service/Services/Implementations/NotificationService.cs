using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Notifications;
using ZiyoNur.Domain.Repositories.Notifications;
using ZiyoNur.Service.Services.Interfaces;

namespace ZiyoNur.Service.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<NotificationService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(
        INotificationRepository notificationRepository,
        ILogger<NotificationService> logger,
        IUnitOfWork unitOfWork)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> SendPushNotificationAsync(string fcmToken, string title, string message, Dictionary<string, string>? data = null)
    {
        try
        {
            // TODO: Implement FCM push notification
            // This is a placeholder implementation
            _logger.LogInformation("Sending push notification to {FcmToken}: {Title}", fcmToken, title);

            // Simulate external API call
            await Task.Delay(100);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending push notification to {FcmToken}", fcmToken);
            return false;
        }
    }

    public async Task<bool> SendBulkPushNotificationAsync(List<string> fcmTokens, string title, string message, Dictionary<string, string>? data = null)
    {
        var tasks = fcmTokens.Select(token => SendPushNotificationAsync(token, title, message, data));
        var results = await Task.WhenAll(tasks);

        return results.All(r => r);
    }

    public async Task CreateInAppNotificationAsync(int userId, string userType, string title, string message, string notificationType, string? imageUrl = null, string? actionUrl = null)
    {
        try
        {
            // Find notification type
            // TODO: Get notification type from repository
            var notification = new Notification
            {
                UserId = userId,
                UserType = userType,
                NotificationTypeId = 1, // TODO: Get from NotificationType repository
                Title = title,
                Message = message,
                ImageUrl = imageUrl,
                ActionUrl = actionUrl,
                IsRead = false,
                IsSent = false
            };

            await _notificationRepository.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("In-app notification created for user {UserId} ({UserType}): {Title}", userId, userType, title);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating in-app notification for user {UserId}", userId);
            throw;
        }
    }

    public async Task<int> GetUnreadCountAsync(int userId, string userType)
    {
        return await _notificationRepository.GetUnreadCountAsync(userId, userType);
    }

    public async Task MarkAsReadAsync(int notificationId, int userId)
    {
        var notification = await _notificationRepository.GetByIdAsync(notificationId);
        if (notification != null && notification.UserId == userId)
        {
            notification.MarkAsRead();
            _notificationRepository.Update(notification);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task MarkAllAsReadAsync(int userId, string userType)
    {
        await _notificationRepository.MarkAllAsReadAsync(userId, userType);
    }
}
