using Stripe;

namespace Customer.API.Repositories.Interfaces;

public interface IStripeCustomerRepository
{
    Task<Stripe.Customer> GetByIdAsync(string id);
    Task<Stripe.Customer> CreateAsync(CustomerCreateOptions options);
}