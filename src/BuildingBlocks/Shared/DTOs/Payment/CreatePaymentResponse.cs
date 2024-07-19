namespace Shared.DTOs.Payment;

public record CreatePaymentResponse(string CheckoutUrl, string CheckoutSessionId, string StripeCustomerId);

public record PaymentCustomerResponse(string Id);
