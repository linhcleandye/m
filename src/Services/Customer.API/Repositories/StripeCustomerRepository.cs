using Customer.API.Repositories.Interfaces;
using Stripe;
using ILogger = Serilog.ILogger;

namespace Customer.API.Repositories;

public class StripeCustomerRepository(ILogger logger, CustomerService stripeCustomerService) : IStripeCustomerRepository
{
    public Task<Stripe.Customer?> GetByIdAsync(string id) => stripeCustomerService.GetAsync(id);

    public Task<Stripe.Customer> CreateAsync(CustomerCreateOptions options)
        => stripeCustomerService.CreateAsync(options);

    public Task<Stripe.Customer> UpdateAsync(string id, CustomerUpdateOptions options)
        => stripeCustomerService.UpdateAsync(id, options);

    public Task<Stripe.Customer> DeleteAsync(string id, CustomerDeleteOptions? options)
        => stripeCustomerService.DeleteAsync(id, options);
}