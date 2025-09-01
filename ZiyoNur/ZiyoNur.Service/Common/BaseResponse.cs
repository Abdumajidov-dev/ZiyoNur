namespace ZiyoNur.Service.Common;

public class BaseResponse<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;

    public static BaseResponse<T> Success(T data, string? message = null)
    {
        return new BaseResponse<T>
        {
            IsSuccess = true,
            Data = data,
            Message = message
        };
    }

    public static BaseResponse<T> Failure(string error)
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Errors = new List<string> { error }
        };
    }

    public static BaseResponse<T> Failure(List<string> errors)
    {
        return new BaseResponse<T>
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}

public class BaseResponse : BaseResponse<object>
{
    public static BaseResponse Success(string? message = null)
    {
        return new BaseResponse
        {
            IsSuccess = true,
            Message = message
        };
    }

    public new static BaseResponse Failure(string error)
    {
        return new BaseResponse
        {
            IsSuccess = false,
            Errors = new List<string> { error }
        };
    }

    public new static BaseResponse Failure(List<string> errors)
    {
        return new BaseResponse
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}
