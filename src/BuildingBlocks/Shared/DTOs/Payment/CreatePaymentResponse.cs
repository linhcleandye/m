namespace Shared.DTOs.Payment;

public record CreatePaymentResponse(string CheckoutUrl, string CheckoutSessionId, string? CustomerId);

public record PaymentCustomerResponse(string Id);
