namespace Shared.DTOs.Payment;

public record CreatePaymentResponse(string CheckoutUrl, string CheckoutSessionId, string StripeCustomerId);

public record PaymentCustomerResponse(string Id);

public record GetCustomerResponse(string Id, bool IsSuccess = true);

public record GetCustomerResponseFailed(string ErrorMessage): GetCustomerResponse(string.Empty, false);
