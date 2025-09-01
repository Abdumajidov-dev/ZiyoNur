using System.ComponentModel.DataAnnotations;
using ZiyoNur.Domain.Common;
using ZiyoNur.Domain.Entities.Orders;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Entities.Payments;

public class PaymentTransaction : BaseAuditableEntity, IHasDomainEvent
{
    public int OrderId { get; set; }

    [MaxLength(50)]
    public string PaymentMethod { get; set; } = string.Empty; // payme, uzcard, click, humo, cash, cashback

    [MaxLength(50)]
    public string? PaymentProvider { get; set; } // payme, uzcard, click, humo

    [MaxLength(200)]
    public string? TransactionId { get; set; } // provider tomonidan berilgan ID

    public decimal Amount { get; set; }
    public decimal CashbackUsed { get; set; } = 0;
    public decimal CashAmount { get; set; } = 0; // aralash to'lovda naqd qism

    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public string? ProviderResponse { get; set; } // provider javobini saqlash

    // Additional payment info
    public string? CardMask { get; set; } // **** **** **** 1234
    public DateTime? ProcessedAt { get; set; }
    public string? FailureReason { get; set; }

    // Navigation Properties
    public virtual Order Order { get; set; } = null!;

    // Domain Events
    public List<BaseEvent> DomainEvents { get; set; } = new();

    // Business Methods
    public bool IsSuccessful => Status == PaymentStatus.Paid;
    public bool IsPending => Status == PaymentStatus.Pending;
    public bool IsFailed => Status == PaymentStatus.Failed;

    public void MarkAsPaid(string? providerTransactionId = null, string? providerResponse = null)
    {
        if (Status == PaymentStatus.Paid)
            throw new InvalidOperationException("Payment is already marked as paid");

        Status = PaymentStatus.Paid;
        ProcessedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(providerTransactionId))
            TransactionId = providerTransactionId;

        if (!string.IsNullOrEmpty(providerResponse))
            ProviderResponse = providerResponse;

        DomainEvents.Add(new Events.Payments.PaymentCompletedEvent(
            Id, OrderId, Amount, PaymentMethod));
    }

    public void MarkAsFailed(string reason, string? providerResponse = null)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        ProcessedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(providerResponse))
            ProviderResponse = providerResponse;

        DomainEvents.Add(new Events.Payments.PaymentFailedEvent(
            Id, OrderId, Amount, reason));
    }

    public void Cancel(string reason = "User cancelled")
    {
        if (Status == PaymentStatus.Paid)
            throw new InvalidOperationException("Cannot cancel completed payment");

        Status = PaymentStatus.Cancelled;
        FailureReason = reason;
        ProcessedAt = DateTime.UtcNow;

        DomainEvents.Add(new Events.Payments.PaymentCancelledEvent(
            Id, OrderId, reason));
    }
}
