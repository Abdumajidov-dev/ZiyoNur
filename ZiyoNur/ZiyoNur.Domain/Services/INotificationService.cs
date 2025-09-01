namespace ZiyoNur.Domain.Services;

public interface INotificationService
{
    Task SendPushNotificationAsync(string fcmToken, string title, string message,
        string? imageUrl = null, Dictionary<string, string>? data = null);
    Task SendBulkPushNotificationAsync(IEnumerable<string> fcmTokens, string title,
        string message, string? imageUrl = null, Dictionary<string, string>? data = null);
    Task CreateNotificationAsync(int userId, string userType, string title, string message,
        int notificationTypeId, string? imageUrl = null, string? actionUrl = null);
    Task CreateBulkNotificationAsync(IEnumerable<int> userIds, string userType,
        string title, string message, int notificationTypeId,
        string? imageUrl = null, string? actionUrl = null);
}
