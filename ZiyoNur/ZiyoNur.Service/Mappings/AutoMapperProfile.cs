using AutoMapper;
using ZiyoNur.Domain.Entities.Delivery;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Entities.Payments;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Service.DTOs.Auth;
using ZiyoNur.Service.DTOs.Orders;
using ZiyoNur.Service.DTOs.Products;
using ZiyoNur.Service.Features.Products.Commands;

namespace ZiyoNur.Service.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateUserMappings();
        CreateProductMappings();
        CreateOrderMappings();
    }

    private void CreateUserMappings()
    {
        // Customer mappings
        CreateMap<Customer, UserDto>()
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "customer"))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => "customer"));

        // Seller mappings  
        CreateMap<Seller, UserDto>()
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "seller"))
            .ForMember(dest => dest.TotalCashback, opt => opt.Ignore());

        // Admin mappings
        CreateMap<Admin, UserDto>()
            .ForMember(dest => dest.UserType, opt => opt.MapFrom(src => "admin"))
            .ForMember(dest => dest.TotalCashback, opt => opt.Ignore());
    }

    private void CreateProductMappings()
    {
        // Product to ProductDto
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.IsLikedByUser, opt => opt.Ignore()) // Set in handler
            .ForMember(dest => dest.IsInUserCart, opt => opt.Ignore()); // Set in handler

        // Product to ProductListDto
        CreateMap<Product, ProductListDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.IsLikedByUser, opt => opt.Ignore()); // Set in handler

        // Category mappings
        CreateMap<Category, CategoryDto>()
            .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories))
            .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));

        // CreateProductCommand to Product
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => Domain.Enums.ProductStatus.Active))
            .ForMember(dest => dest.CreatedById, opt => opt.MapFrom(src => src.CreatedById))
            .ForMember(dest => dest.CreatedByType, opt => opt.MapFrom(src => "admin"));
    }

    private void CreateOrderMappings()
    {
        // Order to OrderDto
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.Seller != null ? src.Seller.FullName : null))
            .ForMember(dest => dest.DiscountReason, opt => opt.MapFrom(src => src.DiscountReason != null ? src.DiscountReason.Name : null))
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
            .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.StatusText, opt => opt.MapFrom(src => GetStatusText(src.Status)))
            .ForMember(dest => dest.DeliveryType, opt => opt.MapFrom(src => src.DeliveryType.ToString()));

        // OrderItem to OrderItemDto
        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl));

        // OrderDelivery to OrderDeliveryDto
        CreateMap<OrderDelivery, OrderDeliveryDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.DeliveryPartnerName, opt => opt.MapFrom(src => src.DeliveryPartner.Name));

        // PaymentTransaction to PaymentTransactionDto
        CreateMap<PaymentTransaction, PaymentTransactionDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }

    private static string GetStatusText(Domain.Enums.OrderStatus status)
    {
        return status switch
        {
            Domain.Enums.OrderStatus.Pending => "Kutilmoqda",
            Domain.Enums.OrderStatus.Confirmed => "Tasdiqlandi",
            Domain.Enums.OrderStatus.Preparing => "Tayyorlanmoqda",
            Domain.Enums.OrderStatus.ReadyForPickup => "Olib ketishga tayyor",
            Domain.Enums.OrderStatus.Shipped => "Jo'natildi",
            Domain.Enums.OrderStatus.Delivered => "Yetkazildi",
            Domain.Enums.OrderStatus.Cancelled => "Bekor qilindi",
            _ => status.ToString()
        };
    }
}
