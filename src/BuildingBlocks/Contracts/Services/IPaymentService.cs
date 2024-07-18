using Shared.DTOs.Payment;

namespace Contracts.Services;

public interface IPaymentService
{
    Task<CreatePaymentResponse> Checkout(CreatePaymentRequest request);
    Task<PaymentCustomerResponse> CreateCustomer(PaymentCustomerRequest request);
}