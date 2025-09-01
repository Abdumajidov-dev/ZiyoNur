using ZiyoNur.Domain.Entities.Payments;
using ZiyoNur.Domain.Enums;

namespace ZiyoNur.Domain.Services;

public interface IPaymentService
{
    Task<PaymentTransaction> InitiatePaymentAsync(int orderId, decimal amount,
        PaymentMethod paymentMethod, string? returnUrl = null);
    Task<PaymentTransaction> ProcessPaymentAsync(string transactionId, string providerResponse);
    Task<PaymentTransaction> RefundPaymentAsync(int paymentTransactionId, decimal amount, string reason);
    Task<bool> VerifyPaymentAsync(string transactionId, string providerTransactionId);
}
