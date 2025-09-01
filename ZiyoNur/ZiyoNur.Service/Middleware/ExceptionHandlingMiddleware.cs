using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using ZiyoNur.Service.Common;

namespace ZiyoNur.Service.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            ValidationException validationEx => new BaseResponse
            {
                IsSuccess = false,
                Message = "Validation failed",
                Errors = validationEx.ValidationErrors
            },
            NotFoundException notFoundEx => new BaseResponse
            {
                IsSuccess = false,
                Message = notFoundEx.Message,
                Errors = new List<string> { notFoundEx.Message }
            },
            UnauthorizedException unauthorizedEx => new BaseResponse
            {
                IsSuccess = false,
                Message = unauthorizedEx.Message,
                Errors = new List<string> { unauthorizedEx.Message }
            },
            ForbiddenException forbiddenEx => new BaseResponse
            {
                IsSuccess = false,
                Message = forbiddenEx.Message,
                Errors = new List<string> { forbiddenEx.Message }
            },
            BusinessRuleException businessRuleEx => new BaseResponse
            {
                IsSuccess = false,
                Message = businessRuleEx.Message,
                Errors = new List<string> { businessRuleEx.Message }
            },
            _ => new BaseResponse
            {
                IsSuccess = false,
                Message = "Tizimda xatolik yuz berdi",
                Errors = new List<string> { "Internal server error" }
            }
        };

        context.Response.StatusCode = GetStatusCode(exception);

        var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            NotFoundException => (int)HttpStatusCode.NotFound,
            UnauthorizedException => (int)HttpStatusCode.Unauthorized,
            ForbiddenException => (int)HttpStatusCode.Forbidden,
            BusinessRuleException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };
    }
}
