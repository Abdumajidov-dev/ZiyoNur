using ZiyoNur.Domain.Entities.Notifications;

namespace ZiyoNur.Domain.Repositories.Notifications;

public interface INotificationRepository : IBaseRepository<Notification>
{
    Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(int userId, string userType);
    Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(int userId, string userType);
    Task<IReadOnlyList<Notification>> GetUnsentNotificationsAsync();
    Task<int> GetUnreadCountAsync(int userId, string userType);
    Task MarkAllAsReadAsync(int userId, string userType);
    Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetPagedNotificationsAsync(
        int userId, string userType, int pageIndex = 0, int pageSize = 20);
}
