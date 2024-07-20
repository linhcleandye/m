using Stripe;

namespace Customer.API.Repositories.Interfaces;

public interface IStripeCustomerRepository
{
    Task<Stripe.Customer?> GetByIdAsync(string id);
    Task<Stripe.Customer> CreateAsync(CustomerCreateOptions options);
    Task<Stripe.Customer> UpdateAsync(string id, CustomerUpdateOptions options);
    Task<Stripe.Customer> DeleteAsync(string id, CustomerDeleteOptions? options);
}