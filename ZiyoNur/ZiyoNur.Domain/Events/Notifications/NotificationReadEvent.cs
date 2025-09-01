using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Notifications;

public class NotificationReadEvent : BaseEvent
{
    public int NotificationId { get; }
    public int UserId { get; }
    public string UserType { get; }

    public NotificationReadEvent(int notificationId, int userId, string userType)
    {
        NotificationId = notificationId;
        UserId = userId;
        UserType = userType;
    }
}
