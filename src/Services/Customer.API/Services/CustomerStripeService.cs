using AutoMapper;
using Customer.API.Exceptions;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Shared.DTOs.Customer.Stripe;

namespace Customer.API.Services;

public class StripeCustomerService(
    IMapper mapper,
    Stripe.CustomerService stripeCustomerService,
    ICustomerRepository repository)
    : Stripe.CustomerService, IStripeCustomerService
{
    public async Task<IResult> GetByIdAsync(string id) => Results.Ok(await GetCustomerByIdAsync(id));

    public async Task<StripeCustomerDto> GetCustomerByIdAsync(string id)
    {
        var stripeCustomer = await stripeCustomerService.GetAsync(id);
        var result = mapper.Map<StripeCustomerDto>(stripeCustomer);
        return result;
    }

    public async Task<IResult> SyncByIdAsync(string id, int customerId)
    {
        var stripeCustomer = await stripeCustomerService.GetAsync(id);
        var customer = await repository.GetByIdAsync(customerId);
        if (customer is null)
            throw new NotFoundException(customerId);

        customer.StripeCustomer = stripeCustomer;
        customer.StripeCustomerId = stripeCustomer.Id;
        
        await repository.UpdateAsync(customer);
        return Results.NoContent();
    }
}