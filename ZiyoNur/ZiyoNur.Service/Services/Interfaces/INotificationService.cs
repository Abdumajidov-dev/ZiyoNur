namespace ZiyoNur.Service.Services.Interfaces;

public interface INotificationService
{
    Task<bool> SendPushNotificationAsync(string fcmToken, string title, string message, Dictionary<string, string>? data = null);
    Task<bool> SendBulkPushNotificationAsync(List<string> fcmTokens, string title, string message, Dictionary<string, string>? data = null);
    Task CreateInAppNotificationAsync(int userId, string userType, string title, string message, string notificationType, string? imageUrl = null, string? actionUrl = null);
    Task<int> GetUnreadCountAsync(int userId, string userType);
    Task MarkAsReadAsync(int notificationId, int userId);
    Task MarkAllAsReadAsync(int userId, string userType);
}
