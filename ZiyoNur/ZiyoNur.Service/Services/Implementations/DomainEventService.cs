using MediatR;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Services;

namespace ZiyoNur.Service.Services.Implementations;

public class DomainEventService : IDomainEventService
{
    private readonly IMediator _mediator;
    private readonly ILogger<DomainEventService> _logger;

    public DomainEventService(IMediator mediator, ILogger<DomainEventService> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task PublishAsync(BaseEvent domainEvent)
    {
        try
        {
            _logger.LogInformation("Publishing domain event: {EventType}", domainEvent.GetType().Name);
            await _mediator.Publish(domainEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing domain event: {EventType}", domainEvent.GetType().Name);
            throw;
        }
    }

    public async Task PublishRangeAsync(IEnumerable<BaseEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await PublishAsync(domainEvent);
        }
    }
}
