using MediatR;
using Microsoft.Extensions.Logging;

namespace ZiyoNur.Service.Behaviors;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestGuid = Guid.NewGuid().ToString();

        var requestNameWithGuid = $"{requestName} [{requestGuid}]";

        _logger.LogInformation("[START] {RequestNameWithGuid}", requestNameWithGuid);

        TResponse response;

        try
        {
            try
            {
                _logger.LogInformation("[PROPS] {RequestNameWithGuid} {Request}", requestNameWithGuid, request);
            }
            catch (NotSupportedException)
            {
                _logger.LogInformation("[Serialization ERROR] {RequestNameWithGuid} Could not serialize the request.", requestNameWithGuid);
            }

            response = await next();

            _logger.LogInformation("[END] {RequestNameWithGuid}", requestNameWithGuid);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[ERROR] {RequestNameWithGuid} {Exception}", requestNameWithGuid, ex.Message);
            throw;
        }

        return response;
    }
}