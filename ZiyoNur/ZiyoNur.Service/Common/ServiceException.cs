namespace ZiyoNur.Service.Common;

public class ServiceException : Exception
{
    public string ErrorCode { get; }
    public Dictionary<string, object> Data { get; }

    public ServiceException(string message, string errorCode = "GENERAL_ERROR")
        : base(message)
    {
        ErrorCode = errorCode;
        Data = new Dictionary<string, object>();
    }

    public ServiceException(string message, Exception innerException, string errorCode = "GENERAL_ERROR")
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Data = new Dictionary<string, object>();
    }

    public ServiceException(string message, string errorCode, Dictionary<string, object> data)
        : base(message)
    {
        ErrorCode = errorCode;
        Data = data;
    }
}

public class ValidationException : ServiceException
{
    public List<string> ValidationErrors { get; }

    public ValidationException(List<string> validationErrors)
        : base("Validation failed", "VALIDATION_ERROR")
    {
        ValidationErrors = validationErrors;
    }

    public ValidationException(string validationError)
        : base("Validation failed", "VALIDATION_ERROR")
    {
        ValidationErrors = new List<string> { validationError };
    }
}

public class NotFoundException : ServiceException
{
    public NotFoundException(string message) : base(message, "NOT_FOUND")
    {
    }

    public NotFoundException(string entityName, object id)
        : base($"{entityName} with id '{id}' was not found", "NOT_FOUND")
    {
    }
}

public class UnauthorizedException : ServiceException
{
    public UnauthorizedException(string message = "Unauthorized") : base(message, "UNAUTHORIZED")
    {
    }
}

public class ForbiddenException : ServiceException
{
    public ForbiddenException(string message = "Access forbidden") : base(message, "FORBIDDEN")
    {
    }
}

public class BusinessRuleException : ServiceException
{
    public BusinessRuleException(string message) : base(message, "BUSINESS_RULE_VIOLATION")
    {
    }
}