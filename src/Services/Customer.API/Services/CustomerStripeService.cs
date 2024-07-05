using AutoMapper;
using Customer.API.Exceptions;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Shared.DTOs.Customer.Stripe;

namespace Customer.API.Services;

public class CustomerStripeService : Stripe.CustomerService, ICustomerStripeService
{
    private readonly Stripe.CustomerService _stripeCustomerService;
    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;

    public CustomerStripeService(IMapper mapper, Stripe.CustomerService stripeCustomerService,
        ICustomerRepository repository)
    {
        _mapper = mapper;
        _stripeCustomerService = stripeCustomerService;
        _repository = repository;
    }
    
    public async Task<IResult> GetByIdAsync(string id)
    {
        var stripeCustomer = await _stripeCustomerService.GetAsync(id);
        var result = _mapper.Map<StripeCustomerDto>(stripeCustomer);

        return Results.Ok(result);
    }

    public async Task<Stripe.Customer> GetCustomerByIdAsync(string id)
    {
        var stripeCustomer = await _stripeCustomerService.GetAsync(id);
        return stripeCustomer;
    }

    public async Task<IResult> SyncByIdAsync(string id, int customerId)
    {
        var stripeCustomer = await _stripeCustomerService.GetAsync(id);
        var customer = await _repository.GetByIdAsync(customerId);
        if (customer is null)
            throw new NotFoundException(customerId);

        customer.StripeCustomer = stripeCustomer;
        customer.StripeCustomerId = stripeCustomer.Id;
        
        await _repository.UpdateAsync(customer);
        return Results.NoContent();
    }
}