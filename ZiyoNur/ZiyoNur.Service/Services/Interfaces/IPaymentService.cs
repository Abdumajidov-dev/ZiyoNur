using ZiyoNur.Service.DTOs.Payments;

namespace ZiyoNur.Service.Services.Interfaces;

public interface IPaymentService
{
    Task<PaymentInitiationResponse> InitiatePaymentAsync(PaymentRequest request);
    Task<PaymentVerificationResponse> VerifyPaymentAsync(string transactionId, string providerTransactionId);
    Task<PaymentRefundResponse> RefundPaymentAsync(string transactionId, decimal amount, string reason);
    Task<List<PaymentMethodDto>> GetAvailablePaymentMethodsAsync();
}