using MediatR;

namespace ZiyoNur.Service.Common;

public abstract class BaseRequest<TResponse> : IRequest<TResponse>
{
    public DateTime RequestedAt { get; } = DateTime.UtcNow;
    public string RequestId { get; } = Guid.NewGuid().ToString();
}

public abstract class BaseRequest : BaseRequest<Unit>
{
}
