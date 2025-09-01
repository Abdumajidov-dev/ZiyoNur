using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZiyoNur.Data.Common;
using ZiyoNur.Data.Configurations.Repositories.Notifications;
using ZiyoNur.Data.Configurations.Repositories.Orders;
using ZiyoNur.Data.Configurations.Repositories.Products;
using ZiyoNur.Data.Configurations.Repositories.Users;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Repositories.Notifications;
using ZiyoNur.Domain.Repositories.Orders;
using ZiyoNur.Domain.Repositories.Products;
using ZiyoNur.Domain.Repositories.Users;

namespace ZiyoNur.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataLayer(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<MarketplaceDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                    builder =>
                    {
                        builder.MigrationsAssembly(typeof(MarketplaceDbContext).Assembly.FullName);
                    });
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        RegisterRepositories(services);

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // User Repositories
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<ISellerRepository, SellerRepository>();
        services.AddScoped<IAdminRepository, AdminRepository>();

        // Product Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Order Repositories
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ICashbackTransactionRepository, CashbackTransactionRepository>();

        // Notification Repositories
        services.AddScoped<INotificationRepository, NotificationRepository>();
    }
}