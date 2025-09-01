using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Notifications;

public class CampaignCancelledEvent : BaseEvent
{
    public int CampaignId { get; }
    public int AdminId { get; }
    public string Reason { get; }

    public CampaignCancelledEvent(int campaignId, int adminId, string reason)
    {
        CampaignId = campaignId;
        AdminId = adminId;
        Reason = reason;
    }
}
