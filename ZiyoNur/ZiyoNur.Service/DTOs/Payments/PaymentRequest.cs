namespace ZiyoNur.Service.DTOs.Payments;

public class PaymentRequest
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // payme, uzcard, click
    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class PaymentInitiationResponse
{
    public bool IsSuccess { get; set; }
    public string? TransactionId { get; set; }
    public string? PaymentUrl { get; set; }
    public string? QrCode { get; set; }
    public string? ErrorMessage { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public class PaymentVerificationResponse
{
    public bool IsSuccess { get; set; }
    public string Status { get; set; } = string.Empty; // success, failed, pending
    public decimal Amount { get; set; }
    public string? ProviderTransactionId { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class PaymentRefundResponse
{
    public bool IsSuccess { get; set; }
    public string? RefundId { get; set; }
    public decimal RefundedAmount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class PaymentMethodDto
{
    public string Code { get; set; } = string.Empty; // payme, uzcard
    public string Name { get; set; } = string.Empty; // Payme, UzCard
    public string? LogoUrl { get; set; }
    public bool IsActive { get; set; }
    public decimal MinAmount { get; set; }
    public decimal MaxAmount { get; set; }
    public string? Description { get; set; }
}