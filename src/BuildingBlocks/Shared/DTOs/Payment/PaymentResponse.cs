namespace Shared.DTOs.Payment;

public record PaymentResponse(
    string Status,
    string PaymentStatus, 
    long? AmountTotal, 
    string CustomerEmail
);