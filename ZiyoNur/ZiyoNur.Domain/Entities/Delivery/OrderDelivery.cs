using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Delivery;

public class OrderDelivery : BaseAuditableEntity, IHasDomainEvent
{
    public int OrderId { get; set; }
    public int DeliveryPartnerId { get; set; }

    [MaxLength(100)]
    public string? TrackingCode { get; set; }

    public DeliveryStatus Status { get; set; } = DeliveryStatus.Assigned;

    [Required]
    public string DeliveryAddress { get; set; } = string.Empty;

    public DateTime? DeliveryDate { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }

    // Delivery details
    public decimal DeliveryFee { get; set; } = 0;
    public string? DeliveryNotes { get; set; }
    public string? RecipientName { get; set; }
    public string? RecipientPhone { get; set; }

    // Navigation Properties
    public virtual Order Order { get; set; } = null!;
    public virtual DeliveryPartner DeliveryPartner { get; set; } = null!;

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public bool IsDelivered => Status == DeliveryStatus.Delivered;
    public bool IsInProgress => Status == DeliveryStatus.InTransit || Status == DeliveryStatus.PickedUp;
    public bool IsOverdue => EstimatedDeliveryDate.HasValue &&
        EstimatedDeliveryDate < DateTime.UtcNow && !IsDelivered;

    public void AssignToPartner(DeliveryPartner partner, string deliveryAddress)
    {
        DeliveryPartnerId = partner.Id;
        DeliveryAddress = deliveryAddress;
        DeliveryFee = partner.DeliveryFee;
        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(partner.EstimatedDeliveryDays);
        Status = DeliveryStatus.Assigned;

        GenerateTrackingCode();

        DomainEvents.Add(new Events.Delivery.DeliveryAssignedEvent(
            Id, OrderId, partner.Id, TrackingCode!));
    }

    public void PickUp(string? notes = null)
    {
        if (Status != DeliveryStatus.Assigned)
            throw new InvalidOperationException("Delivery is not in assigned status");

        Status = DeliveryStatus.PickedUp;
        DeliveryNotes = notes;

        DomainEvents.Add(new Events.Delivery.DeliveryPickedUpEvent(
            Id, OrderId, TrackingCode!));
    }

    public void MarkInTransit(string? notes = null)
    {
        if (Status != DeliveryStatus.PickedUp)
            throw new InvalidOperationException("Delivery must be picked up first");

        Status = DeliveryStatus.InTransit;
        if (!string.IsNullOrEmpty(notes))
            DeliveryNotes += $"\n{DateTime.UtcNow:yyyy-MM-dd HH:mm}: {notes}";

        DomainEvents.Add(new Events.Delivery.DeliveryInTransitEvent(
            Id, OrderId, TrackingCode!));
    }

    public void Complete(string? notes = null)
    {
        Status = DeliveryStatus.Delivered;
        DeliveryDate = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(notes))
            DeliveryNotes += $"\n{DateTime.UtcNow:yyyy-MM-dd HH:mm}: {notes}";

        // Update partner statistics
        DeliveryPartner.UpdateDeliveryStats(true);

        // Update order status
        Order.ChangeStatus(OrderStatus.Delivered, DeliveryPartnerId, "delivery_partner",
            "Delivered by delivery partner");

        DomainEvents.Add(new Events.Delivery.DeliveryCompletedEvent(
            Id, OrderId, TrackingCode!, DeliveryDate.Value));
    }

    public void MarkAsFailed(string reason)
    {
        Status = DeliveryStatus.Failed;
        DeliveryNotes += $"\n{DateTime.UtcNow:yyyy-MM-dd HH:mm}: FAILED - {reason}";

        // Update partner statistics
        DeliveryPartner.UpdateDeliveryStats(false);

        DomainEvents.Add(new Events.Delivery.DeliveryFailedEvent(
            Id, OrderId, TrackingCode!, reason));
    }

    private void GenerateTrackingCode()
    {
        // Generate unique tracking code: ORD{OrderId}-DEL{DeliveryId}-{Random}
        var random = new Random().Next(1000, 9999);
        TrackingCode = $"ORD{OrderId}-DEL{Id}-{random}";
    }
}
