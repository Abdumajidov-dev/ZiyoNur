namespace ZiyoNur.Service.Utils;

public static class Constants
{
    public static class Roles
    {
        public const string Customer = "customer";
        public const string Seller = "seller";
        public const string Admin = "admin";
        public const string SuperAdmin = "super_admin";
    }

    public static class UserTypes
    {
        public const string Customer = "customer";
        public const string Seller = "seller";
        public const string Admin = "admin";
    }

    public static class NotificationTypes
    {
        public const string OrderCreated = "order_created";
        public const string OrderStatusChanged = "order_status_changed";
        public const string CashbackEarned = "cashback_earned";
        public const string Promotion = "promotion";
        public const string News = "news";
        public const string ProductOutOfStock = "product_out_of_stock";
    }

    public static class PaymentMethods
    {
        public const string Payme = "payme";
        public const string Uzcard = "uzcard";
        public const string Click = "click";
        public const string Humo = "humo";
        public const string Cash = "cash";
        public const string Cashback = "cashback";
        public const string Mixed = "mixed";
    }

    public static class OrderStatuses
    {
        public const string Pending = "pending";
        public const string Confirmed = "confirmed";
        public const string Preparing = "preparing";
        public const string ReadyForPickup = "ready_for_pickup";
        public const string Shipped = "shipped";
        public const string Delivered = "delivered";
        public const string Cancelled = "cancelled";
    }

    public static class CacheKeys
    {
        public const string Products = "products";
        public const string Categories = "categories";
        public const string SystemSettings = "system_settings";
        public const string PaymentMethods = "payment_methods";

        public static string ProductById(int id) => $"product:{id}";
        public static string CategoryById(int id) => $"category:{id}";
        public static string CustomerById(int id) => $"customer:{id}";
        public static string UserNotifications(int userId, string userType) => $"notifications:{userType}:{userId}";
    }

    public static class FileExtensions
    {
        public static readonly string[] Images = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        public static readonly string[] Documents = { ".pdf", ".doc", ".docx", ".txt", ".xlsx", ".xls" };
        public static readonly string[] AllowedExtensions = Images.Concat(Documents).ToArray();
    }
}