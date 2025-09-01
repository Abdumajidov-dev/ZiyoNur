using FluentValidation;
using MediatR;
using ZiyoNur.Service.Common;
using ValidationException = ZiyoNur.Service.Common.ValidationException;

namespace ZiyoNur.Service.Behaviors;
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            var errorMessages = failures.Select(f => f.ErrorMessage).ToList();

            // If TResponse is BaseResponse<T>, return validation error response
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(BaseResponse<>))
            {
                var responseType = typeof(TResponse);
                var failureMethod = responseType.GetMethod("Failure", new[] { typeof(List<string>) });
                if (failureMethod != null)
                {
                    return (TResponse)failureMethod.Invoke(null, new object[] { errorMessages })!;
                }
            }

            throw new ValidationException(errorMessages);
        }

        return await next();
    }
}