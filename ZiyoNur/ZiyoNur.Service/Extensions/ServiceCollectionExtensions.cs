using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using ZiyoNur.Domain.Services;
using ZiyoNur.Service.BackgroundServices;
using ZiyoNur.Service.Behaviors;
using ZiyoNur.Service.Services.Implementations;
using ZiyoNur.Service.Services.Interfaces;
using IEmailService = ZiyoNur.Service.Services.Interfaces.IEmailService;
using IFileService = ZiyoNur.Service.Services.Interfaces.IFileService;
using INotificationService = ZiyoNur.Service.Services.Interfaces.INotificationService;
using IPaymentService = ZiyoNur.Service.Services.Interfaces.IPaymentService;
using ISmsService = ZiyoNur.Service.Services.Interfaces.ISmsService;

namespace ZiyoNur.Service.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServiceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // MediatR for CQRS
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // FluentValidation
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // AutoMapper
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Application Services
        RegisterApplicationServices(services, configuration);

        // Domain Services (if implemented in Service layer)
        RegisterDomainServices(services);

        // Background Services
        services.AddHostedService<NotificationBackgroundService>();

        // MediatR Pipeline Behaviors
        RegisterPipelineBehaviors(services);

        return services;
    }

    private static void RegisterApplicationServices(IServiceCollection services, IConfiguration configuration)
    {
        // Authentication & Authorization
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        // Notification Services
        services.AddScoped<INotificationService, NotificationService>();

        // File Services
        services.AddScoped<IFileService, FileService>();

        // External Services
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<IPaymentService, PaymentService>();

        // HTTP Clients for external APIs
        services.AddHttpClient("PaymeClient", client =>
        {
            client.BaseAddress = new Uri(configuration["ExternalServices:Payme:BaseUrl"] ?? "https://checkout.paycom.uz/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        services.AddHttpClient("FCMClient", client =>
        {
            client.BaseAddress = new Uri("https://fcm.googleapis.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
        });
    }

    private static void RegisterDomainServices(IServiceCollection services)
    {
        // These should be implemented in Service layer
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ICacheService, MemoryCacheService>();
        services.AddScoped<IDomainEventService, DomainEventService>();
    }

    private static void RegisterPipelineBehaviors(IServiceCollection services)
    {
        // Validation behavior
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Logging behavior
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        // Performance behavior
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
    }
}
