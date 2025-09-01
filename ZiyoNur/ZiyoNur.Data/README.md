# Data Layer - README.md

## 📋 Umumiy Ma'lumot

**Data Layer** bu loyihamizning ma'lumotlar bazasi bilan ishlash qismi. Bu layer Domain Layer'dagi entity'larni haqiqiy database jadvallariga aylantiradi va barcha ma'lumotlarni saqlash/o'qish jarayonlarini boshqaradi.

## 🎯 Data Layer Nima Uchun Kerak?

- **Ma'lumotlarni xavfsiz saqlash**: SQL Server database'da
- **Performance optimization**: Index'lar va query optimization
- **Data integrity**: Foreign key'lar va constraint'lar
- **Audit trail**: Kim, qachon, nimani o'zgartirgan
- **Migration**: Database schema'ni versiyalash
- **Seed data**: Boshlang'ich ma'lumotlar bilan to'ldirish

## 🏗️ Data Layer Tuzilmasi

```
Data/
├── MarketplaceDbContext.cs     # Asosiy DbContext
├── Common/                     # Umumiy komponentlar
│   ├── BaseRepository.cs      # Asosiy CRUD amallar
│   └── UnitOfWork.cs          # Transaction boshqaruvi
├── Configurations/            # Entity sozlamalari
│   ├── Users/                # Foydalanuvchi jadval sozlamalari
│   │   ├── CustomerConfiguration.cs
│   │   ├── SellerConfiguration.cs
│   │   └── AdminConfiguration.cs
│   ├── Products/             # Mahsulot jadval sozlamalari
│   │   ├── ProductConfiguration.cs
│   │   ├── CategoryConfiguration.cs
│   │   ├── CartConfiguration.cs
│   │   └── ProductLikeConfiguration.cs
│   ├── Orders/               # Buyurtma jadval sozlamalari
│   ├── Payments/             # To'lov jadval sozlamalari
│   ├── Delivery/             # Yetkazib berish sozlamalari
│   ├── Notifications/        # Xabar sozlamalari
│   ├── Support/              # Support sozlamalari
│   ├── Content/              # Kontent sozlamalari
│   ├── Reports/              # Hisobot sozlamalari
│   └── System/               # Tizim sozlamalari
├── Repositories/             # Repository implementatsiyalari
│   ├── Users/
│   ├── Products/
│   ├── Orders/
│   └── Notifications/
├── Seed/                     # Boshlang'ich ma'lumotlar
│   └── DatabaseSeeder.cs
├── Extensions/               # DI va konfiguratsiya
│   └── ServiceCollectionExtensions.cs
├── Interceptors/             # Audit va logging
│   └── AuditInterceptor.cs
└── Migrations/               # EF Core migration'lar
```

## 🧩 Asosiy Komponentlar

### 1. **MarketplaceDbContext** - Asosiy Database Context

```csharp
public class MarketplaceDbContext : DbContext
{
    // DbSet'lar - har bir jadval uchun
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    // ... va boshqalar
    
    // Audit fields avtomatik yangilash
    public override async Task<int> SaveChangesAsync()
    {
        UpdateAuditFields(); // CreatedAt, UpdatedAt
        await DispatchDomainEvents(); // Domain eventlarni jo'natish
        return await base.SaveChangesAsync();
    }
}
```

**Vazifalar:**
- **Jadval yaratish**: Entity'larni database jadvallariga aylantirish
- **Audit**: Avtomatik CreatedAt, UpdatedAt to'ldirish
- **Soft Delete**: DeletedAt ni to'ldirish
- **Domain Events**: Eventlarni Service Layer'ga jo'natish

### 2. **Entity Configurations** - Jadval Sozlamalari

**Nima bu:** Har bir entity uchun database jadval sozlamalarini belgilash.

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Jadval nomi
        builder.ToTable("Products");
        
        // Primary key
        builder.HasKey(x => x.Id);
        
        // Maydon sozlamalari
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(x => x.Price)
            .HasPrecision(18, 2); // decimal uchun
            
        // Relationship'lar
        builder.HasOne(x => x.Category)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId);
            
        // Index'lar - tez qidiruv uchun
        builder.HasIndex(x => x.QrCode)
            .IsUnique();
            
        builder.HasIndex(x => x.Name); // Search uchun
    }
}
```

**Nima uchun kerak:**
- **Maydon uzunligi**: varchar(200), nvarchar(max)
- **Relationship'lar**: Foreign key'lar
- **Index'lar**: Tez qidiruv
- **Constraint'lar**: Unique, Required
- **Default qiymatlar**: IsActive = true

### 3. **Repository Pattern** - Ma'lumot Olish/Saqlash

```csharp
public class ProductRepository : BaseRepository<Product>, IProductRepository
{
    public async Task<Product?> GetByQrCodeAsync(string qrCode)
    {
        return await _dbSet
            .Include(p => p.Category) // Category ma'lumotini ham ol
            .FirstOrDefaultAsync(x => x.QrCode == qrCode);
    }
    
    public async Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm)
    {
        var normalized = searchTerm.ToLower().Trim();
        
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.Name.ToLower().Contains(normalized) ||
                       p.Description.ToLower().Contains(normalized))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
}
```

**Repository'ning foydalar:**
- **Abstraction**: Database'dan mustaqil
- **Reusability**: Bir necha joyda ishlatish
- **Testing**: Mock repository yaratish oson
- **Complex queries**: Business logic bilan birga

### 4. **BaseRepository<T>** - Umumiy CRUD Amallar

```csharp
public class BaseRepository<TEntity> : IBaseRepository<TEntity>
{
    // Asosiy amallar
    Task<TEntity?> GetByIdAsync(int id);
    Task<IReadOnlyList<TEntity>> GetAllAsync();
    Task<TEntity> AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Delete(TEntity entity);
    Task SoftDeleteAsync(int id); // DeletedAt = DateTime.Now
    
    // Pagination
    Task<(IReadOnlyList<TEntity> Items, int TotalCount)> GetPagedAsync();
    
    // Search
    Task<IReadOnlyList<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate);
}
```

### 5. **Unit of Work** - Transaction Boshqaruvi

```csharp
public class UnitOfWork : IUnitOfWork
{
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    
    public async Task BeginTransactionAsync() { ... }
    public async Task CommitTransactionAsync() { ... }
    public async Task RollbackTransactionAsync() { ... }
}
```

**Vazifasi:**
- **Transaction**: Bir nechta amallarni birgalikda bajarish
- **Rollback**: Xatolik bo'lsa, hamma o'zgarishni bekor qilish
- **Consistency**: Ma'lumotlar izchilligi

## 📚 Ishlatilgan Kutubxonalar

### **Microsoft.EntityFrameworkCore (8.0.0)**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
```

**Nima uchun:**
- **ORM**: Object-Relational Mapping
- **Code First**: C# code'dan database yaratish
- **Migration**: Database schema versiyalash
- **LINQ**: C# da SQL yozish
- **Change Tracking**: O'zgarishlarni kuzatish

### **BCrypt.Net-Next (4.0.3)**
```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
```

**Nima uchun:**
- **Password hashing**: Parollarni xavfsiz saqlash
- **Salt**: Har bir parol uchun unique salt
- **Brute force protection**: Sekin hashing algorithm

## 🌱 Seed Data - Boshlang'ich Ma'lumotlar

### **System Settings**
```csharp
new SystemSetting 
{ 
    SettingKey = "Cashback.Percentage", 
    SettingValue = "2.0",
    Description = "Default cashback percentage" 
}
```

**10+ sistema sozlamalari:**
- App nomi va versiyasi
- Cashback foizi va muddati
- Do'kon manzili va telefoni
- Yetkazib berish sozlamalari

### **Test Foydalanuvchilar**
```csharp
// Admin
Username: admin
Password: Admin123!

// Manager  
Username: manager
Password: Manager123!

// Sellers
Phone: +998903333333
Password: Seller123!

// Customers
Phone: +998905555555
Password: Customer123!
```

### **Mahsulotlar va Kategoriyalar**
```
Kitoblar/
├── Diniy kitoblar
│   ├── Qur'oni Karim - 85,000 so'm
│   ├── Sahih Buxoriy - 120,000 so'm
│   └── Islom tarixidan lavhalar - 65,000 so'm
├── Darsliklar
│   ├── Matematika 10-sinf - 45,000 so'm
│   └── Ingliz tili grammatikasi - 38,000 so'm

Diniy liboslar/
├── Erkaklar uchun
│   ├── Erkaklar jubba (oq) - 180,000 so'm
│   └── Erkaklar do'ppisi - 35,000 so'm
├── Ayollar uchun
│   ├── Ayollar abayasi (qora) - 220,000 so'm
│   └── Ayollar hijabi - 25,000 so'm

Ofis buyumlari/
├── Yozuv qurollari
│   ├── Ruchka (ko'k) - 2,500 so'm
│   └── Qalam to'plami - 15,000 so'm

Sovg'alar/
├── Tasbehlar
│   ├── Yog'och tasbeh - 45,000 so'm
│   └── Marjon tasbeh - 85,000 so'm
```

### **Discount Reasons**
- Doimiy mijoz
- Aksiya
- Nuqsonli mahsulot
- Xodim chegirmasi
- Ommaviy xarid
- Tug'ilgan kun

### **Delivery Partners**
- O'zbekiston Pochtasi - 15,000 so'm, 2 kun
- Express Pochta - 25,000 so'm, 1 kun
- Tez Kuryer - 20,000 so'm, 1 kun

## 🚀 Database Performance Optimizations

### **Index'lar - Tez Qidiruv**
```sql
-- Customers
CREATE INDEX IX_Customers_Phone ON Customers(Phone)
CREATE INDEX IX_Customers_Email ON Customers(Email)

-- Products  
CREATE INDEX IX_Products_QrCode ON Products(QrCode)
CREATE INDEX IX_Products_Name ON Products(Name)
CREATE INDEX IX_Products_CategoryId ON Products(CategoryId)

-- Orders
CREATE INDEX IX_Orders_CustomerId ON Orders(CustomerId)
CREATE INDEX IX_Orders_OrderDate ON Orders(OrderDate)
CREATE INDEX IX_Orders_Status ON Orders(Status)

-- Composite indexes
CREATE INDEX IX_Orders_Customer_Status ON Orders(CustomerId, Status)
CREATE INDEX IX_Notifications_User_Read ON Notifications(UserId, UserType, IsRead)
```

### **Query Optimization**
- **Include()**: Related data'ni bir query'da olish
- **AsNoTracking()**: Read-only query'lar uchun
- **Pagination**: Katta ma'lumotlarni bo'laklash
- **Projection**: Faqat kerakli maydonlarni olish

### **Connection Pooling**
```csharp
services.AddDbContext<MarketplaceDbContext>(options =>
{
    options.UseSqlServer(connectionString, builder =>
    {
        builder.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });
});
```

## 🔐 Security Features

### **Soft Delete - Global Query Filter**
```csharp
// Avtomatik ravishda o'chirilgan yozuvlar ko'rsatilmaydi
modelBuilder.Entity<BaseEntity>()
    .HasQueryFilter(e => e.DeletedAt == null);
```

### **SQL Injection Protection**
- **Parameterized queries**: EF Core avtomatik qiladi
- **No raw SQL**: Repository'larda to'g'ridan-to'g'ri SQL yo'q
- **Input validation**: Domain layer'da validation

### **Audit Trail**
```csharp
foreach (var entry in ChangeTracker.Entries<BaseEntity>())
{
    switch (entry.State)
    {
        case EntityState.Added:
            entry.Entity.CreatedAt = DateTime.UtcNow;
            break;
        case EntityState.Modified:
            entry.Entity.UpdatedAt = DateTime.UtcNow;
            break;
        case EntityState.Deleted:
            entry.Entity.DeletedAt = DateTime.UtcNow;
            entry.State = EntityState.Modified; // Soft delete
            break;
    }
}
```

## 📊 Migration Commands

### **Development jarayonida**
```bash
# Yangi migration yaratish
dotnet ef migrations add CreateInitialTables --project Data --startup-project Api

# Database'ni yangilash
dotnet ef database update --project Data --startup-project Api

# Oxirgi migration'ni o'chirish
dotnet ef migrations remove --project Data --startup-project Api

# Migration script yaratish
dotnet ef migrations script --project Data --startup-project Api
```

### **Production**
```bash
# Faqat script yaratish
dotnet ef migrations script --idempotent --output migrate.sql

# Production database'da SQL script'ni ishga tushirish
sqlcmd -S server -d database -i migrate.sql
```

## 🧪 Testing Data Layer

### **Repository Testing**
```csharp
[Test]
public async Task GetProductByQrCode_ShouldReturnProduct()
{
    // Arrange
    using var context = CreateInMemoryContext();
    var repository = new ProductRepository(context);
    
    var product = new Product { Name = "Test", QrCode = "TEST001" };
    await context.Products.AddAsync(product);
    await context.SaveChangesAsync();
    
    // Act
    var result = await repository.GetByQrCodeAsync("TEST001");
    
    // Assert
    Assert.NotNull(result);
    Assert.AreEqual("Test", result.Name);
    Assert.AreEqual("TEST001", result.QrCode);
}
```

### **Integration Testing**
```csharp
[Test]
public async Task OrderCreation_ShouldTriggerCashbackCalculation()
{
    // Test'da haqiqiy database jarayonlarini tekshirish
    using var context = CreateTestDatabase();
    
    // Order yaratish
    var order = new Order { CustomerId = 1, TotalPrice = 100000 };
    await context.Orders.AddAsync(order);
    await context.SaveChangesAsync();
    
    // Cashback yig'ilganini tekshirish
    var cashback = await context.CashbackTransactions
        .FirstOrDefaultAsync(c => c.OrderId == order.Id);
        
    Assert.NotNull(cashback);
    Assert.AreEqual(2000, cashback.Amount); // 2% from 100000
}
```

## ⚡ Performance Monitoring

### **Slow Query Detection**
```csharp
public class AuditInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.SavingChangesAsync(eventData, result, cancellationToken);
        stopwatch.Stop();
        
        if (stopwatch.ElapsedMilliseconds > 1000) // 1 sekund
        {
            _logger.LogWarning("Slow database operation: {ElapsedMs}ms", 
                stopwatch.ElapsedMilliseconds);
        }
        
        return result;
    }
}
```

### **Database Statistics**
- **Connection pooling**: Aktiv ulanishlar soni
- **Query performance**: Eng sekin query'lar
- **Index usage**: Qaysi index'lar ishlatilmaydi
- **Table sizes**: Eng katta jadvallar

## 🔄 Database Backup Strategy

### **Development**
```bash
# Database backup
sqlcmd -S localhost -Q "BACKUP DATABASE MarketplaceDb TO DISK='C:\backup\marketplace.bak'"

# Restore
sqlcmd -S localhost -Q "RESTORE DATABASE MarketplaceDb FROM DISK='C:\backup\marketplace.bak'"
```

### **Production**
- **Daily backup**: Har kuni avtomatik
- **Point-in-time recovery**: Istalgan vaqtga qaytarish
- **Offsite backup**: Boshqa joyda saqlash
- **Backup verification**: Backup'ning to'g'riligini tekshirish

## 🚨 Error Handling

### **Connection Failures**
```csharp
services.AddDbContext<MarketplaceDbContext>(options =>
{
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,              // 3 marta urinish
            maxRetryDelay: TimeSpan.FromSeconds(30), // 30 sekund kutish
            errorNumbersToAdd: null        // Barcha xatoliklar uchun
        );
    });
});
```

### **Transaction Failures**
```csharp
public async Task CreateOrderWithItems(Order order, List<OrderItem> items)
{
    await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        await _orderRepository.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        foreach (var item in items)
        {
            item.OrderId = order.Id;
            await _orderItemRepository.AddAsync(item);
        }
        
        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitTransactionAsync();
    }
    catch
    {
        await _unitOfWork.RollbackTransactionAsync();
        throw; // Xatolikni yuqoriga uzatish
    }
}
```

## 📈 Monitoring va Logging

### **EF Core Logging**
```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging() // Faqat development'da
        .EnableDetailedErrors();
}
```

### **Query Logging**
- **Executed queries**: Qaysi SQL bajarilyapti
- **Parameters**: Qaysi parametrlar uzatilyapti
- **Execution time**: Har bir query qancha vaqt oladi
- **Connection events**: Ulanish ochl/yopilishi

### **Application Insights**
```csharp
services.AddApplicationInsightsTelemetry();
services.AddDbContext<MarketplaceDbContext>(options =>
{
    options.UseSqlServer(connectionString)
           .AddInterceptors(new ApplicationInsightsInterceptor());
});
```

## 🔧 Configuration Settings

### **appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MarketplaceDb;Trusted_Connection=true;TrustServerCertificate=true;"
  },
  "DatabaseSettings": {
    "EnableSensitiveDataLogging": false,
    "CommandTimeout": 30,
    "MaxRetryCount": 3,
    "MaxRetryDelay": "00:00:30"
  }
}
```

### **Environment Variables**
```bash
# Development
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection="..."

# Production
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection="..."
```

## 🎯 Data Layer Best Practices

### **Repository Pattern Do's:**
✅ **IReadOnlyList qaytarish** - Collection'ni o'zgartirib bo'lmaydi  
✅ **Async/await ishlatish** - Non-blocking operations  
✅ **Include() bilan related data** - N+1 problem'ni oldini olish  
✅ **Pagination qo'llab-quvvatlash** - Katta ma'lumotlar uchun  
✅ **CancellationToken** - Operation'ni bekor qilish imkoniyati  

### **Repository Pattern Don'ts:**
❌ **IQueryable qaytarish** - Repository abstraction buziladi  
❌ **SaveChanges() repository'da** - Bu UnitOfWork vazifasi  
❌ **Business logic repository'da** - Bu Domain Layer vazifasi  
❌ **Exception handling repository'da** - Service Layer'da qilinishi kerak  

### **Performance Do's:**
✅ **AsNoTracking()** - Read-only query'lar uchun  
✅ **Select() projection** - Faqat kerakli maydonlar  
✅ **Batch operations** - Ko'p ma'lumot uchun  
✅ **Connection pooling** - Ulanishlarni qayta ishlatish  

### **Security Do's:**
✅ **Parameterized queries** - SQL injection'dan himoya  
✅ **Soft delete** - Ma'lumotlarni butunlay o'chirmaslik  
✅ **Audit fields** - Kim, qachon o'zgartirgan  
✅ **Input validation** - Domain Layer'da  

## 🔗 Boshqa Layer'lar Bilan Bog'lanish

### **Domain Layer bilan:**
```csharp
// Data Layer Domain'dagi interface'larni implement qiladi
public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
{
    // Domain'dagi ICustomerRepository'ni implement qilish
}
```

### **Service Layer bilan:**
```csharp
// Service Layer Data Layer'dan foydalanadi
public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public OrderService(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
    }
}
```

### **API Layer bilan:**
```csharp
// API Layer'da Data Layer register qilinadi
public void ConfigureServices(IServiceCollection services)
{
    services.AddDataLayer(Configuration);
    // Repository'lar avtomatik DI container'ga qo'shiladi
}
```

## 📋 Checklist - Data Layer Tayyor

### **Database Schema:**
- [x] Barcha jadvallar yaratildi (15+ jadval)
- [x] Foreign key'lar to'g'ri o'rnatildi
- [x] Index'lar performance uchun qo'shildi
- [x] Unique constraint'lar qo'yildi
- [x] Default qiymatlar belgilandi

### **Repository'lar:**
- [x] BaseRepository CRUD amallar
- [x] Specialized repository'lar (Customer, Product, Order)
- [x] Complex query'lar (search, filter, pagination)
- [x] Async/await pattern
- [x] Unit of Work pattern

### **Configuration:**
- [x] Entity configuration'lar
- [x] Fluent API mapping'lar
- [x] Enum conversion'lar
- [x] Precision settings
- [x] Relationship configuration'lar

### **Seed Data:**
- [x] System settings
- [x] Admin foydalanuvchilar
- [x] Test ma'lumotlar
- [x] Kategoria hierarchy
- [x] Sample products

### **Performance:**
- [x] Database index'lar
- [x] Query optimization
- [x] Connection pooling
- [x] Retry policies
- [x] Audit interceptor

## ❗ Muhim Eslatmalar

1. **Migration'larni production'da ehtiyotkorlik bilan bajaring**
2. **Sensitive data'ni log'larga yozmaslik (production'da)**
3. **Database backup'larni muntazam olish**
4. **Performance monitoring o'rnatish**
5. **Connection string'larni xavfsiz saqlash**
6. **Test environment'da seed data ishlatish**

## 🎉 Natija

Data Layer tayyor bo'lgandan so'ng:
- ✅ **30+ jadval** SQL Server'da yaratiladi
- ✅ **50+ relationship** to'g'ri ishlaydi
- ✅ **Repository pattern** business logic'dan ajratildi
- ✅ **Performance optimized** query'lar
- ✅ **Audit trail** barcha o'zgarishlar yoziladi
- ✅ **Test data** development uchun tayyor

Bu layer loyihaning 25% qismini tashkil etadi va barcha ma'lumotlar xavfsiz, tez va ishonchli saqlanadi! 💾🚀