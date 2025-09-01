using Microsoft.EntityFrameworkCore;
using ZiyoNur.Data.Common;
using ZiyoNur.Domain.Entities.Notifications;
using ZiyoNur.Domain.Repositories.Notifications;

namespace ZiyoNur.Data.Configurations.Repositories.Notifications;

public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
{
    public NotificationRepository(MarketplaceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Notification>> GetUserNotificationsAsync(int userId, string userType)
    {
        return await _dbSet
            .Include(n => n.NotificationType)
            .Where(n => n.UserId == userId && n.UserType == userType)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Notification>> GetUnreadNotificationsAsync(int userId, string userType)
    {
        return await _dbSet
            .Include(n => n.NotificationType)
            .Where(n => n.UserId == userId && n.UserType == userType && !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Notification>> GetUnsentNotificationsAsync()
    {
        return await _dbSet
            .Include(n => n.NotificationType)
            .Where(n => !n.IsSent && n.NotificationType.IsActive)
            .OrderBy(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetUnreadCountAsync(int userId, string userType)
    {
        return await _dbSet
            .CountAsync(n => n.UserId == userId && n.UserType == userType && !n.IsRead);
    }

    public async Task MarkAllAsReadAsync(int userId, string userType)
    {
        var unreadNotifications = await _dbSet
            .Where(n => n.UserId == userId && n.UserType == userType && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkAsRead();
        }

        await _context.SaveChangesAsync();
    }

    public async Task<(IReadOnlyList<Notification> Items, int TotalCount)> GetPagedNotificationsAsync(
        int userId, string userType, int pageIndex = 0, int pageSize = 20)
    {
        IQueryable<Notification> query = _dbSet
            .Include(n => n.NotificationType)
            .Where(n => n.UserId == userId && n.UserType == userType);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}