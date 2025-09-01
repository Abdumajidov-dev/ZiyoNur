# Domain Layer - README.md

## 📋 Umumiy Ma'lumot

**Domain Layer** bu bizning loyihamizning yurak qismi bo'lib, barcha biznes qoidalari va mantiqiy jarayonlarni o'z ichiga oladi. Bu layer hech qanday texnologiyaga bog'liq emas va faqat bizning kutubxona marketplace'imizning qoidalarini ifodalaydi.

## 🎯 Domain Layer Nima Uchun Kerak?

- **Biznes mantiqini himoya qilish**: Barcha qoidalar bir joyda
- **Mustaqillik**: Boshqa layerlardan bog'liq emas
- **Qayta ishlatish**: Har qanday texnologiya bilan ishlaydi
- **Test qilish**: Oson test yozish imkoniyati
- **Kengaytirish**: Yangi biznes qoidalari qo'shish oson

## 🏗️ Domain Layer Tuzilmasi

```
Domain/
├── Common/                 # Asosiy abstract classlar
│   ├── BaseEntity.cs      # Barcha entitylar uchun asosiy class
│   ├── BaseAuditableEntity.cs  # Audit maydonlari bilan
│   ├── BaseEvent.cs       # Domain eventlar uchun
│   ├── IHasDomainEvent.cs # Domain event interface
│   └── Result.cs          # Natija qaytarish uchun
├── Entities/              # Bizning asosiy obyektlarimiz
│   ├── Users/            # Foydalanuvchilar
│   │   ├── Customer.cs   # Mijozlar
│   │   ├── Seller.cs     # Sotuvchilar  
│   │   └── Admin.cs      # Administratorlar
│   ├── Products/         # Mahsulotlar
│   │   ├── Product.cs    # Mahsulot
│   │   ├── Category.cs   # Kategoriya
│   │   ├── Cart.cs       # Savat
│   │   └── ProductLike.cs # Yoqtirish
│   ├── Orders/           # Buyurtmalar
│   │   ├── Order.cs      # Buyurtma
│   │   ├── OrderItem.cs  # Buyurtma mahsuloti
│   │   ├── CashbackTransaction.cs # Cashback
│   │   └── DiscountReason.cs # Chegirma sababi
│   ├── Payments/         # To'lovlar
│   ├── Delivery/         # Yetkazib berish
│   ├── Notifications/    # Xabarlar
│   ├── Support/          # Qo'llab-quvvatlash
│   └── Content/          # Kontent
├── Enums/                # Enum'lar (holatlar)
├── Events/               # Domain eventlar
├── Repositories/         # Repository interface'lari
└── Services/             # Domain service'lari
```

## 🧩 Asosiy Komponentlar

### 1. **BaseEntity** - Asosiy Entity Class
```csharp
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
```

**Nima uchun kerak:**
- Har bir jadval uchun umumiy maydonlar
- Soft Delete (ma'lumotni o'chirmasdan belgilash)
- Audit trail (qachon yaratilgan/o'zgartirilgan)

### 2. **Domain Events** - Voqealar Tizimi

**Nima bu:** Biror muhim narsa bo'lganda avtomatik ravishda boshqa jarayonlarni ishga tushirish.

**Misollar:**
```csharp
public class OrderCreatedEvent : BaseEvent
{
    public int OrderId { get; }
    public int CustomerId { get; }
    // Yangi buyurtma yaratilganda
}

public class CashbackEarnedEvent : BaseEvent
{
    public int CustomerId { get; }
    public decimal Amount { get; }
    // Cashback yig'ilganda
}
```

**Foydasi:**
- **Tizim bog'lanmasligi**: Bir qism boshqasini bilmaydi
- **Avtomatlashtirish**: Buyurtma tayyor bo'lsa, avtomatik xabar yuboriladi
- **Kengaytirish**: Yangi voqea handler'larni qo'shish oson
- **Monitoring**: Barcha voqealarni kuzatish mumkin

### 3. **Rich Domain Models** - Boy Domain Modellari

**Oddiy model (yomon):**
```csharp
public class Order
{
    public decimal TotalPrice { get; set; }
    public decimal DiscountApplied { get; set; }
}
```

**Rich model (yaxshi):**
```csharp
public class Order
{
    public decimal TotalPrice { get; private set; }
    public decimal DiscountApplied { get; private set; }
    
    // Biznes logikasi
    public decimal FinalPrice => TotalPrice - DiscountApplied;
    public bool CanBeCancelled => Status == OrderStatus.Pending;
    
    // Biznes amallar
    public void ApplyDiscount(decimal amount, int reasonId)
    {
        if (amount < 0 || amount > TotalPrice)
            throw new ArgumentException("Noto'g'ri chegirma summasi");
            
        DiscountApplied = amount;
        // Domain event ishga tushadi
    }
}
```

**Foydasi:**
- **Xavfsizlik**: Noto'g'ri ma'lumot kiritish imkonsiz
- **Mantiq bir joyda**: Barcha qoidalar entity ichida
- **O'qish oson**: Kod o'qish va tushunish oson

## 📊 Asosiy Entity'lar Tushuntirishi

### **Customer (Mijoz)**
- **Vazifasi**: Mijozlar ma'lumotlarini saqlash
- **Xususiyatlari**: FCM token (push notification), cashback balansi
- **Biznes qoidalari**: Cashback ishlatish, savatga qo'shish

### **Product (Mahsulot)**
- **Vazifasi**: Mahsulotlar ma'lumoti
- **Xususiyatlari**: QR kod, zaxira miqdori, status
- **Biznes qoidalari**: Zaxira tekshirish, narx o'zgartirish

### **Order (Buyurtma)**
- **Vazifasi**: Buyurtmalarni boshqarish  
- **Xususiyatlari**: Status, to'lov usuli, yetkazib berish
- **Biznes qoidalari**: Chegirma qo'llash, status o'zgartirish, cashback hisoblash

### **Category (Kategoriya)**
- **Vazifasi**: Mahsulotlarni guruhlash
- **Xususiyatlari**: Hierarhik struktura (ota-bola)
- **Biznes qoidalari**: To'liq yo'lni ko'rsatish, mahsulotlar mavjudligini tekshirish

## 🔄 Domain Events Ro'yxati

### **User Events (Foydalanuvchi voqealari)**
- `CustomerRegisteredEvent`: Yangi mijoz ro'yxatdan o'tdi
- `CustomerLoginEvent`: Mijoz tizimga kirdi

### **Product Events (Mahsulot voqealari)**
- `ProductStockUpdatedEvent`: Mahsulot zaxirasi o'zgartirildi
- `ProductPriceChangedEvent`: Mahsulot narxi o'zgartirildi
- `ProductOutOfStockEvent`: Mahsulot tugab qoldi

### **Order Events (Buyurtma voqealari)**
- `OrderCreatedEvent`: Yangi buyurtma yaratildi
- `OrderStatusChangedEvent`: Buyurtma holati o'zgartirildi
- `OrderCancelledEvent`: Buyurtma bekor qilindi
- `CashbackEarnedEvent`: Cashback yig'ildi

### **Payment Events (To'lov voqealari)**
- `PaymentCompletedEvent`: To'lov muvaffaqiyatli amalga oshdi
- `PaymentFailedEvent`: To'lov muvaffaqiyatsiz tugadi

### **Delivery Events (Yetkazib berish voqealari)**
- `DeliveryAssignedEvent`: Buyurtma yetkazib berish xizmatiga berildi
- `DeliveryCompletedEvent`: Yetkazib berish yakunlandi

## 🎛️ Enum'lar (Holatlar)

```csharp
public enum OrderStatus
{
    Pending = 1,        // Kutilmoqda
    Confirmed = 2,      // Tasdiqlandi
    Preparing = 3,      // Tayyorlanmoqda
    ReadyForPickup = 4, // Olib ketishga tayyor
    Shipped = 5,        // Jo'natildi
    Delivered = 6,      // Yetkazildi
    Cancelled = 7       // Bekor qilindi
}
```

**Nima uchun int qiymat:** Database'da saqlash oson, performance yaxshi.

## 🔌 Repository Pattern

**Nima bu:** Database bilan ishlashni abstract qilish.

```csharp
public interface IProductRepository : IBaseRepository<Product>
{
    Task<Product?> GetByQrCodeAsync(string qrCode);
    Task<IReadOnlyList<Product>> SearchProductsAsync(string searchTerm);
    Task<IReadOnlyList<Product>> GetLowStockProductsAsync(int threshold);
}
```

**Foydasi:**
- **Test qilish oson**: Mock repository yaratish mumkin
- **Database mustaqil**: SQL Server, PostgreSQL, MongoDB - farqi yo'q
- **Kengaytirish oson**: Yangi query'lar qo'shish oson

## 🛡️ Business Rules (Biznes Qoidalari)

### **Cashback Qoidalari:**
- Har xariddan 2% cashback
- 30 kun davomida amal qiladi
- FIFO tartibida ishlatiladi (eskisi birinchi)

### **Discount Qoidalari:**
- Eng arzon mahsulotdan boshlab qo'llaniladi
- Jami summadan ko'p bo'lishi mumkin emas
- Sababini ko'rsatish majburiy

### **Order Qoidalari:**
- Faqat Pending va Confirmed holatdagi buyurtmalarni bekor qilish mumkin
- Delivered holatiga o'tganda avtomatik cashback hisoblanadi
- Zaxira tekshirilmaydi (order item qo'shganda)

## 🧪 Test Examples (Test Misollari)

```csharp
[Test]
public void Order_ApplyDiscount_ShouldDistributeCorrectly()
{
    // Arrange - Tayyorlash
    var order = new Order();
    order.AddItem(product1, quantity: 2, unitPrice: 2000); // 4000 so'm
    order.AddItem(product2, quantity: 1, unitPrice: 50000); // 50000 so'm
    
    // Act - Harakat
    order.ApplyDiscount(10000, discountReasonId: 1);
    
    // Assert - Tekshirish
    var cheaperItem = order.OrderItems.OrderBy(x => x.UnitPrice).First();
    Assert.AreEqual(4000, cheaperItem.DiscountApplied); // To'liq
    
    var expensiveItem = order.OrderItems.OrderBy(x => x.UnitPrice).Last();
    Assert.AreEqual(6000, expensiveItem.DiscountApplied); // Qolgan
}
```

## 📚 Qo'shimcha Ma'lumotlar

### **Design Patterns Ishlatilgan:**
- **Repository Pattern**: Database abstraction
- **Domain Events Pattern**: Loosely coupled communication
- **Rich Domain Model**: Business logic in entities
- **Specification Pattern**: Complex queries
- **Result Pattern**: Error handling

### **SOLID Principles:**
- **S**: Har bir class bitta vazifaga javobgar
- **O**: Extension uchun ochiq, modification uchun yopiq
- **L**: Base class o'rniga derived class ishlatish mumkin
- **I**: Interface segregation
- **D**: Abstraksiyalarga bog'lanish, konkret class'larga emas

### **Clean Architecture Benefits:**
- **Testable**: Har bir qismni alohida test qilish mumkin
- **Independent**: Framework'lardan mustaqil
- **UI Independent**: Console, Web, Mobile - farqi yo'q
- **Database Independent**: SQL, NoSQL - farqi yo'q

## ❗ Muhim Eslatmalar

1. **Domain Layer hech qanday dependency'larga bog'liq emas**
2. **Barcha business logic shu layerda bo'lishi kerak**
3. **Database, UI, HTTP - bularni Domain bilmasligi kerak**
4. **Events faqat Domain ichida generate qilinadi**
5. **Validation Domain va Application layerlarda ikki xil**

## 🔄 Migration va Development

Domain o'zgarganda:
1. **Entity o'zgartiriladi**
2. **Migration yaratiladi**
3. **Repository'lar yangilanadi** 
4. **Test'lar yoziladi**
5. **Documentation yangilanadi**

Bu layerni tushunish loyihaning 70% qismini tushunish demakdir! 🎯