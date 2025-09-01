using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Notifications;

public class CampaignScheduledEvent : BaseEvent
{
    public int CampaignId { get; }
    public int AdminId { get; }
    public DateTime ScheduledAt { get; }

    public CampaignScheduledEvent(int campaignId, int adminId, DateTime scheduledAt)
    {
        CampaignId = campaignId;
        AdminId = adminId;
        ScheduledAt = scheduledAt;
    }
}
