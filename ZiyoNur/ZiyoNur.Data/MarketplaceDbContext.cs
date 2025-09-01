using Microsoft.EntityFrameworkCore;
using System.Reflection;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Content;
using ZiyoNur.Domain.Entities.Delivery;
using ZiyoNur.Domain.Entities.Notifications;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Entities.Payments;
using ZiyoNur.Domain.Entities.Products;
using ZiyoNur.Domain.Entities.Support;
using ZiyoNur.Domain.Entities.System;
using ZiyoNur.Domain.Entities.Users;
using ZiyoNur.Domain.Reports;

namespace ZiyoNur.Data;

public class MarketplaceDbContext : DbContext
{
    public MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options) : base(options)
    {
    }

    // User DbSets
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Seller> Sellers { get; set; }
    public DbSet<Admin> Admins { get; set; }

    // Product DbSets
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<ProductLike> ProductLikes { get; set; }

    // Order DbSets
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<DiscountReason> DiscountReasons { get; set; }
    public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
    public DbSet<CashbackTransaction> CashbackTransactions { get; set; }
    public DbSet<CashbackSetting> CashbackSettings { get; set; }

    // Payment & Delivery DbSets
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
    public DbSet<DeliveryPartner> DeliveryPartners { get; set; }
    public DbSet<OrderDelivery> OrderDeliveries { get; set; }

    // Notification DbSets
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationType> NotificationTypes { get; set; }
    public DbSet<AdminNotificationCampaign> AdminNotificationCampaigns { get; set; }

    // Support & Content DbSets
    public DbSet<SupportChat> SupportChats { get; set; }
    public DbSet<Content> Contents { get; set; }

    // Report & System DbSets
    public DbSet<SalesReport> SalesReports { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global filters for soft delete
        ApplyGlobalFilters(modelBuilder);

        // Configure decimal precision
        ConfigureDecimalProperties(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update audit fields
        UpdateAuditFields();

        // Dispatch domain events
        await DispatchDomainEvents();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void UpdateAuditFields()
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    entry.Property(x => x.CreatedAt).IsModified = false;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.DeletedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }

    private async Task DispatchDomainEvents()
    {
        var domainEventEntities = ChangeTracker.Entries<IHasDomainEvent>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEventEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEventEntities.ForEach(entity => entity.Entity.DomainEvents.Clear());

        // Domain events will be handled by MediatR in Service layer
        // For now, just clear them
        foreach (var domainEvent in domainEvents)
        {
            // TODO: Publish domain event using MediatR
            // await _mediator.Publish(domainEvent);
        }
    }

    private void ApplyGlobalFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(MarketplaceDbContext)
                    .GetMethod(nameof(SetGlobalFilter), BindingFlags.NonPublic | BindingFlags.Static)?
                    .MakeGenericMethod(entityType.ClrType);
                method?.Invoke(null, new object[] { modelBuilder });
            }
        }
    }

    private static void SetGlobalFilter<T>(ModelBuilder modelBuilder) where T : BaseEntity
    {
        modelBuilder.Entity<T>().HasQueryFilter(e => e.DeletedAt == null);
    }

    private void ConfigureDecimalProperties(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var properties = entityType.ClrType.GetProperties()
                .Where(p => p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?));

            foreach (var property in properties)
            {
                modelBuilder.Entity(entityType.Name)
                    .Property(property.Name)
                    .HasPrecision(18, 2);
            }
        }
    }
}
