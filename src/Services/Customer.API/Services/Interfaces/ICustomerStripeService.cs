using Shared.DTOs.Customer.Stripe;

namespace Customer.API.Services.Interfaces;

public interface IStripeCustomerService
{
    Task<IResult> GetByIdAsync(string id);
    Task<StripeCustomerDto> GetCustomerByIdAsync(string id);
    Task<IResult> SyncByIdAsync(string id, int customerId);
}