using Customer.API.Repositories.Interfaces;
using Stripe;

namespace Customer.API.Repositories;

public class StripeCustomerRepository(CustomerService stripeCustomerService) : IStripeCustomerRepository
{
    public Task<Stripe.Customer> GetByIdAsync(string id) => stripeCustomerService.GetAsync(id);

    public Task<Stripe.Customer> CreateAsync(CustomerCreateOptions options) =>
        stripeCustomerService.CreateAsync(options);
}