using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Events.Notifications;

public class CampaignSentEvent : BaseEvent
{
    public int CampaignId { get; }
    public int AdminId { get; }
    public int Recipients { get; }

    public CampaignSentEvent(int campaignId, int adminId, int recipients)
    {
        CampaignId = campaignId;
        AdminId = adminId;
        Recipients = recipients;
    }
}
