using Shared.DTOs.Payment;

namespace Contracts.Services;

public interface IPaymentService
{
    Task<CreatePaymentResponse> CheckoutAsync(CreatePaymentRequest request);
    Task<PaymentResponse> GetCheckoutSessionStatusAsync(string sessionId);
}