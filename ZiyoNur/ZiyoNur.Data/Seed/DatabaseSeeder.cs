using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZiyoNur.Domain.Entities.Delivery;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Entities.System;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Entities.Notifications;
using ZiyoNur.Domain.Enums;
using BCrypt.Net;
using NotificationType = ZiyoNur.Domain.Entities.Notifications.NotificationType;

namespace ZiyoNur.Data.Seed;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MarketplaceDbContext>>();

        try
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Run migrations if needed
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
            }

            // Seed data in order
            await SeedSystemSettings(context, logger);
            await SeedNotificationTypes(context, logger);
            await SeedAdmins(context, logger);
            await SeedSellers(context, logger);
            await SeedCustomers(context, logger);
            await SeedCategories(context, logger);
            await SeedProducts(context, logger);
            await SeedDiscountReasons(context, logger);
            await SeedDeliveryPartners(context, logger);
            await SeedCashbackSettings(context, logger);

            await context.SaveChangesAsync();
            logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private static async Task SeedSystemSettings(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.SystemSettings.AnyAsync()) return;

        var settings = new[]
        {
                new SystemSetting { SettingKey = "App.Name", SettingValue = "Kutubxona Marketplace", Description = "Application name", DataType = "string", IsPublic = true },
                new SystemSetting { SettingKey = "App.Version", SettingValue = "1.0.0", Description = "Application version", DataType = "string", IsPublic = true },
                new SystemSetting { SettingKey = "Cashback.Percentage", SettingValue = "2.0", Description = "Default cashback percentage", DataType = "decimal" },
                new SystemSetting { SettingKey = "Cashback.ValidityDays", SettingValue = "30", Description = "Cashback validity in days", DataType = "integer" },
                new SystemSetting { SettingKey = "Order.AutoConfirmMinutes", SettingValue = "30", Description = "Auto confirm order in minutes", DataType = "integer" },
                new SystemSetting { SettingKey = "Notifications.EnablePush", SettingValue = "true", Description = "Enable push notifications", DataType = "boolean", IsPublic = true },
                new SystemSetting { SettingKey = "Store.Phone", SettingValue = "+998901234567", Description = "Store contact phone", DataType = "string", IsPublic = true },
                new SystemSetting { SettingKey = "Store.Address", SettingValue = "Toshkent shahar, Chilonzor tumani", Description = "Store address", DataType = "string", IsPublic = true },
                new SystemSetting { SettingKey = "Store.WorkingHours", SettingValue = "9:00-18:00", Description = "Store working hours", DataType = "string", IsPublic = true },
                new SystemSetting { SettingKey = "Delivery.FreeOrderThreshold", SettingValue = "100000", Description = "Free delivery threshold amount", DataType = "decimal", IsPublic = true }
            };

        await context.SystemSettings.AddRangeAsync(settings);
        logger.LogInformation("System settings seeded");
    }

    private static async Task SeedNotificationTypes(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.NotificationTypes.AnyAsync()) return;

        var notificationTypes = new[]
        {
                new NotificationType
                {
                    Name = "order_created",
                    Description = "New order created",
                    DefaultTitle = "Yangi buyurtma",
                    DefaultMessage = "Yangi buyurtma #{OrderId} yaratildi",
                    SendPushNotification = true,
                    SendInApp = true
                },
                new NotificationType
                {
                    Name = "order_status_changed",
                    Description = "Order status updated",
                    DefaultTitle = "Buyurtma holati o'zgartirildi",
                    DefaultMessage = "Buyurtmangiz #{OrderId} holati: {Status}",
                    SendPushNotification = true,
                    SendInApp = true
                },
                new NotificationType
                {
                    Name = "cashback_earned",
                    Description = "Cashback earned",
                    DefaultTitle = "Cashback yig'ildingiz!",
                    DefaultMessage = "{Amount} so'm cashback hisobingizga qo'shildi",
                    SendPushNotification = true,
                    SendInApp = true
                },
                new NotificationType
                {
                    Name = "promotion",
                    Description = "Promotional notifications",
                    DefaultTitle = "Maxsus taklif!",
                    DefaultMessage = "Yangi aksiya va chegirmalar haqida xabar",
                    SendPushNotification = true,
                    SendInApp = true
                },
                new NotificationType
                {
                    Name = "news",
                    Description = "News and updates",
                    DefaultTitle = "Yangiliklar",
                    DefaultMessage = "Kutubxona yangiliklari",
                    SendPushNotification = false,
                    SendInApp = true
                }
            };

        await context.NotificationTypes.AddRangeAsync(notificationTypes);
        logger.LogInformation("Notification types seeded");
    }

    private static async Task SeedAdmins(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.Admins.AnyAsync()) return;

        var admins = new[]
        {
                new Admin
                {
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    FirstName = "System",
                    LastName = "Administrator",
                    Email = "admin@kutubxona.uz",
                    Phone = "+998901111111",
                    Role = "super_admin",
                    IsActive = true,
                    CanManageProducts = true,
                    CanManageOrders = true,
                    CanManageUsers = true,
                    CanViewReports = true,
                    CanManageNotifications = true
                },
                new Admin
                {
                    Username = "manager",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Manager123!"),
                    FirstName = "Kutubxona",
                    LastName = "Manager",
                    Email = "manager@kutubxona.uz",
                    Phone = "+998902222222",
                    Role = "manager",
                    IsActive = true,
                    CanManageProducts = true,
                    CanManageOrders = true,
                    CanManageUsers = false,
                    CanViewReports = true,
                    CanManageNotifications = true
                }
            };

        await context.Admins.AddRangeAsync(admins);
        logger.LogInformation("Admins seeded");
    }

    private static async Task SeedSellers(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.Sellers.AnyAsync()) return;

        var sellers = new[]
        {
                new Seller
                {
                    FirstName = "Oybek",
                    LastName = "Karimov",
                    Phone = "+998903333333",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Seller123!"),
                    Role = "senior_seller",
                    IsActive = true
                },
                new Seller
                {
                    FirstName = "Malika",
                    LastName = "Ibragimova",
                    Phone = "+998904444444",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Seller123!"),
                    Role = "seller",
                    IsActive = true
                }
            };

        await context.Sellers.AddRangeAsync(sellers);
        logger.LogInformation("Sellers seeded");
    }

    private static async Task SeedCustomers(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.Customers.AnyAsync()) return;

        var customers = new[]
        {
                new Customer
                {
                    FirstName = "Aziz",
                    LastName = "Rajabov",
                    Phone = "+998905555555",
                    Email = "aziz@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                    Address = "Toshkent, Yunusobod tumani, 12-uy",
                    IsActive = true,
                    TotalCashback = 15000
                },
                new Customer
                {
                    FirstName = "Nilufar",
                    LastName = "Usmanova",
                    Phone = "+998906666666",
                    Email = "nilufar@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                    Address = "Toshkent, Chilonzor tumani, 25-uy",
                    IsActive = true,
                    TotalCashback = 8500
                },
                new Customer
                {
                    FirstName = "Sardor",
                    LastName = "Toshev",
                    Phone = "+998907777777",
                    Email = "sardor@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer123!"),
                    Address = "Toshkent, Mirzo Ulug'bek tumani, 8-uy",
                    IsActive = true,
                    TotalCashback = 22000
                }
            };

        await context.Customers.AddRangeAsync(customers);
        logger.LogInformation("Customers seeded");
    }

    private static async Task SeedCategories(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.Categories.AnyAsync()) return;

        // Root categories
        var rootCategories = new[]
        {
                new Category { Name = "Kitoblar", Description = "Turli xil kitoblar", IsActive = true, SortOrder = 1 },
                new Category { Name = "Diniy liboslar", Description = "Islom diniy libosu", IsActive = true, SortOrder = 2 },
                new Category { Name = "Sovg'alar", Description = "Turli sovg'alar", IsActive = true, SortOrder = 3 },
                new Category { Name = "Ofis buyumlari", Description = "Ish uchun kerakli buyumlar", IsActive = true, SortOrder = 4 }
            };

        await context.Categories.AddRangeAsync(rootCategories);
        await context.SaveChangesAsync();

        // Get IDs for subcategories
        var kitoblarCategory = await context.Categories.FirstAsync(c => c.Name == "Kitoblar");
        var diniyLibosCategory = await context.Categories.FirstAsync(c => c.Name == "Diniy liboslar");
        var sovgalarCategory = await context.Categories.FirstAsync(c => c.Name == "Sovg'alar");
        var ofisCategory = await context.Categories.FirstAsync(c => c.Name == "Ofis buyumlari");

        // Subcategories
        var subCategories = new[]
        {
                // Kitoblar subcategories
                new Category { Name = "Diniy kitoblar", ParentId = kitoblarCategory.Id, IsActive = true, SortOrder = 1 },
                new Category { Name = "Darsliklar", ParentId = kitoblarCategory.Id, IsActive = true, SortOrder = 2 },
                new Category { Name = "Badiiy adabiyot", ParentId = kitoblarCategory.Id, IsActive = true, SortOrder = 3 },
                new Category { Name = "Bolalar kitoblari", ParentId = kitoblarCategory.Id, IsActive = true, SortOrder = 4 },

                // Diniy liboslar subcategories
                new Category { Name = "Erkaklar uchun", ParentId = diniyLibosCategory.Id, IsActive = true, SortOrder = 1 },
                new Category { Name = "Ayollar uchun", ParentId = diniyLibosCategory.Id, IsActive = true, SortOrder = 2 },
                new Category { Name = "Bolalar uchun", ParentId = diniyLibosCategory.Id, IsActive = true, SortOrder = 3 },

                // Sovg'alar subcategories
                new Category { Name = "Tasbehlar", ParentId = sovgalarCategory.Id, IsActive = true, SortOrder = 1 },
                new Category { Name = "Dekorativ buyumlar", ParentId = sovgalarCategory.Id, IsActive = true, SortOrder = 2 },
                new Category { Name = "Parfyumeriya", ParentId = sovgalarCategory.Id, IsActive = true, SortOrder = 3 },

                // Ofis subcategories
                new Category { Name = "Yozuv qurollari", ParentId = ofisCategory.Id, IsActive = true, SortOrder = 1 },
                new Category { Name = "Daftarlar", ParentId = ofisCategory.Id, IsActive = true, SortOrder = 2 },
                new Category { Name = "Kancelярiya", ParentId = ofisCategory.Id, IsActive = true, SortOrder = 3 }
            };

        await context.Categories.AddRangeAsync(subCategories);
        logger.LogInformation("Categories seeded");
    }

    private static async Task SeedProducts(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.Products.AnyAsync()) return;

        // Get category IDs
        var diniyKitoblarCategory = await context.Categories.FirstAsync(c => c.Name == "Diniy kitoblar");
        var darsliklarCategory = await context.Categories.FirstAsync(c => c.Name == "Darsliklar");
        var erkakLibosCategory = await context.Categories.FirstAsync(c => c.Name == "Erkaklar uchun");
        var ayollarLibosCategory = await context.Categories.FirstAsync(c => c.Name == "Ayollar uchun");
        var yozuvQurallariCategory = await context.Categories.FirstAsync(c => c.Name == "Yozuv qurollari");
        var tasbehlarCategory = await context.Categories.FirstAsync(c => c.Name == "Tasbehlar");

        var products = new[]
        {
                // Diniy kitoblar
                new Product
                {
                    Name = "Qur'oni Karim (Tarjima bilan)",
                    Description = "O'zbek tilidagi tarjimasi bilan Qur'oni Karim",
                    Price = 85000,
                    Count = 50,
                    CategoryId = diniyKitoblarCategory.Id,
                    QrCode = "QK001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "quran, qur'on, kitob, diniy"
                },
                new Product
                {
                    Name = "Sahih Buxoriy",
                    Description = "Imam Buxoriyning mashhur hadis to'plami",
                    Price = 120000,
                    Count = 25,
                    CategoryId = diniyKitoblarCategory.Id,
                    QrCode = "SB001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "buxoriy, hadis, diniy kitob"
                },
                new Product
                {
                    Name = "Islom tarixidan lavhalar",
                    Description = "Islom tarixidagi muhim voqealar haqida",
                    Price = 65000,
                    Count = 35,
                    CategoryId = diniyKitoblarCategory.Id,
                    QrCode = "IT001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "tarix, islom, diniy"
                },

                // Darsliklar
                new Product
                {
                    Name = "Matematika 10-sinf",
                    Description = "10-sinf uchun matematika darsligi",
                    Price = 45000,
                    Count = 100,
                    CategoryId = darsliklarCategory.Id,
                    QrCode = "M10001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "matematika, darslik, 10-sinf"
                },
                new Product
                {
                    Name = "Ingliz tili grammatikasi",
                    Description = "Ingliz tili grammatikasi bo'yicha qo'llanma",
                    Price = 38000,
                    Count = 75,
                    CategoryId = darsliklarCategory.Id,
                    QrCode = "EN001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "ingliz tili, grammatika, til"
                },

                // Erkaklar libosu
                new Product
                {
                    Name = "Erkaklar jubba (oq)",
                    Description = "Sifatli matoda tikilgan oq rangli erkaklar jubbasi",
                    Price = 180000,
                    Count = 20,
                    CategoryId = erkakLibosCategory.Id,
                    QrCode = "EJ001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "jubba, erkak, oq, kiyim"
                },
                new Product
                {
                    Name = "Erkaklar do'ppisi",
                    Description = "An'anaviy o'zbek do'ppisi",
                    Price = 35000,
                    Count = 45,
                    CategoryId = erkakLibosCategory.Id,
                    QrCode = "ED001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "doppi, erkak, bosh kiyim"
                },

                // Ayollar libosu
                new Product
                {
                    Name = "Ayollar abayasi (qora)",
                    Description = "Sifatli qora rangli ayollar abayasi",
                    Price = 220000,
                    Count = 15,
                    CategoryId = ayollarLibosCategory.Id,
                    QrCode = "AA001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "abaya, ayol, qora, kiyim"
                },
                new Product
                {
                    Name = "Ayollar hijabi",
                    Description = "Turli rangdagi ayollar hijabi",
                    Price = 25000,
                    Count = 80,
                    CategoryId = ayollarLibosCategory.Id,
                    QrCode = "AH001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "hijab, ayol, bosh yopish"
                },

                // Yozuv qurollari
                new Product
                {
                    Name = "Ruchka (ko'k)",
                    Description = "Sifatli ko'k rangli ruchka",
                    Price = 2500,
                    Count = 200,
                    CategoryId = yozuvQurallariCategory.Id,
                    QrCode = "RK001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "ruchka, yozish, qalam, ko'k"
                },
                new Product
                {
                    Name = "Qalam to'plami",
                    Description = "Rangli qalamlar to'plami (12 ta)",
                    Price = 15000,
                    Count = 60,
                    CategoryId = yozuvQurallariCategory.Id,
                    QrCode = "QT001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "qalam, rangli, chizma, art"
                },

                // Tasbehlar
                new Product
                {
                    Name = "Yog'och tasbeh",
                    Description = "Sifatli yog'ochdan yasalgan tasbeh (99 dona)",
                    Price = 45000,
                    Count = 30,
                    CategoryId = tasbehlarCategory.Id,
                    QrCode = "YT001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "tasbeh, yog'och, zikr, diniy"
                },
                new Product
                {
                    Name = "Marjon tasbeh",
                    Description = "Marjondan yasalgan tasbeh",
                    Price = 85000,
                    Count = 15,
                    CategoryId = tasbehlarCategory.Id,
                    QrCode = "MT001",
                    Status = ProductStatus.Active,
                    SearchKeywords = "tasbeh, marjon, zikr, qimmat"
                }
            };

        await context.Products.AddRangeAsync(products);
        logger.LogInformation("Products seeded");
    }

    private static async Task SeedDiscountReasons(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.DiscountReasons.AnyAsync()) return;

        var discountReasons = new[]
        {
                new DiscountReason { Name = "Doimiy mijoz", Description = "Doimiy mijozlarga beriladigan chegirma", IsActive = true },
                new DiscountReason { Name = "Aksiya", Description = "Maxsus aksiya chegirmasi", IsActive = true },
                new DiscountReason { Name = "Nuqsonli mahsulot", Description = "Nuqsoni bor mahsulotga chegirma", IsActive = true },
                new DiscountReason { Name = "Xodim chegirmasi", Description = "Kutubxona xodimlari uchun", IsActive = true },
                new DiscountReason { Name = "Ommaviy xarid", Description = "Ko'p miqdorda xarid qilganlarga", IsActive = true },
                new DiscountReason { Name = "Tug'ilgan kun", Description = "Tug'ilgan kun munosabati bilan", IsActive = true }
            };

        await context.DiscountReasons.AddRangeAsync(discountReasons);
        logger.LogInformation("Discount reasons seeded");
    }

    private static async Task SeedDeliveryPartners(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.DeliveryPartners.AnyAsync()) return;

        var deliveryPartners = new[]
        {
                new DeliveryPartner
                {
                    Name = "O'zbekiston Pochtasi",
                    Phone = "+998712345678",
                    Type = "pochta",
                    Address = "Toshkent shahar, A.Temur ko'chasi",
                    DeliveryFee = 15000,
                    EstimatedDeliveryDays = 2,
                    IsActive = true,
                    ServiceAreas = "Toshkent, Samarqand, Buxoro"
                },
                new DeliveryPartner
                {
                    Name = "Express Pochta",
                    Phone = "+998901234567",
                    Type = "pochta",
                    Address = "Toshkent shahar, Navoi ko'chasi",
                    DeliveryFee = 25000,
                    EstimatedDeliveryDays = 1,
                    IsActive = true,
                    ServiceAreas = "Toshkent viloyati"
                },
                new DeliveryPartner
                {
                    Name = "Tez Kuryer",
                    Phone = "+998909876543",
                    Type = "kuryer",
                    Address = "Toshkent shahar",
                    DeliveryFee = 20000,
                    EstimatedDeliveryDays = 1,
                    IsActive = true,
                    ServiceAreas = "Toshkent shahar"
                }
            };

        await context.DeliveryPartners.AddRangeAsync(deliveryPartners);
        logger.LogInformation("Delivery partners seeded");
    }

    private static async Task SeedCashbackSettings(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.CashbackSettings.AnyAsync()) return;

        var cashbackSetting = new CashbackSetting
        {
            CashbackPercentage = 2.0m,
            ValidityPeriodDays = 30,
            MinimumOrderAmount = 10000,
            IsActive = true
        };

        await context.CashbackSettings.AddAsync(cashbackSetting);
        logger.LogInformation("Cashback settings seeded");
    }

    public static async Task SeedTestData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<MarketplaceDbContext>>();

        try
        {
            await SeedTestOrders(context, logger);
            await context.SaveChangesAsync();
            logger.LogInformation("Test data seeding completed successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding test data");
            throw;
        }
    }

    private static async Task SeedTestOrders(MarketplaceDbContext context, ILogger logger)
    {
        if (await context.Orders.AnyAsync()) return;

        // Get some test data
        var customer = await context.Customers.FirstAsync();
        var seller = await context.Sellers.FirstAsync();
        var product1 = await context.Products.FirstAsync(p => p.QrCode == "QK001");
        var product2 = await context.Products.FirstAsync(p => p.QrCode == "RK001");
        var discountReason = await context.DiscountReasons.FirstAsync(d => d.Name == "Doimiy mijoz");

        // Create a test order
        var order = new Order
        {
            CustomerId = customer.Id,
            SellerId = seller.Id,
            OrderDate = DateTime.UtcNow.AddDays(-5),
            TotalPrice = 90000,
            DiscountApplied = 5000,
            DiscountReasonId = discountReason.Id,
            PaymentMethod = PaymentMethod.Cash,
            PaymentStatus = PaymentStatus.Paid,
            Status = OrderStatus.Delivered,
            DeliveryType = DeliveryType.Pickup,
            OrderSource = "pos_system"
        };

        await context.Orders.AddAsync(order);
        await context.SaveChangesAsync();

        // Add order items
        var orderItems = new[]
        {
                new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product1.Id,
                    Quantity = 1,
                    UnitPrice = 85000,
                    TotalPrice = 85000,
                    DiscountApplied = 5000
                },
                new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product2.Id,
                    Quantity = 2,
                    UnitPrice = 2500,
                    TotalPrice = 5000,
                    DiscountApplied = 0
                }
            };

        await context.OrderItems.AddRangeAsync(orderItems);

        // Add cashback transaction
        var cashbackTransaction = new CashbackTransaction
        {
            CustomerId = customer.Id,
            OrderId = order.Id,
            Amount = 1700, // 2% of 85000
            Type = CashbackTransactionType.Earned,
            ExpiryDate = DateTime.UtcNow.AddDays(30),
            IsUsed = false
        };

        await context.CashbackTransactions.AddAsync(cashbackTransaction);

        logger.LogInformation("Test orders seeded");
    }
}