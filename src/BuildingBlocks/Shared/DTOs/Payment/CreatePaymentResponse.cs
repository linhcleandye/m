namespace Shared.DTOs.Payment;

public record CreatePaymentResponse(string CheckoutUrl, string CheckoutSessionId);