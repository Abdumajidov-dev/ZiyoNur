using ZiyoNur.Domain.Common;

namespace ZiyoNur.Domain.Services;

public interface IDomainEventService
{
    Task PublishAsync(BaseEvent domainEvent);
    Task PublishRangeAsync(IEnumerable<BaseEvent> domainEvents);
}
