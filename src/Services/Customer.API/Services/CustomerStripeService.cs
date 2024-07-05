using AutoMapper;
using Customer.API.Entities;
using Customer.API.Exceptions;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Shared.DTOs.Customer.Stripe;

namespace Customer.API.Services;

public class CustomerStripeService : Stripe.CustomerService, ICustomerStripeService
{
    private readonly Stripe.CustomerService _stripeCustomerService;
    private readonly ICustomerService _customerService;
    private readonly ICustomerRepository _repository;
    private readonly IMapper _mapper;

    public CustomerStripeService(IMapper mapper, Stripe.CustomerService stripeCustomerService,
        ICustomerRepository repository,
        ICustomerService customerService)
    {
        _mapper = mapper;
        _stripeCustomerService = stripeCustomerService;
        _customerService = customerService;
        _repository = repository;
    }
    
    public async Task<IResult> GetByIdAsync(string id)
    {
        var stripeCustomer = await _stripeCustomerService.GetAsync(id);
        var result = _mapper.Map<StripeCustomerDto>(stripeCustomer);

        return Results.Ok(result);
    }

    public async Task<IResult> SyncByIdAsync(string id, int customerId)
    {
        var stripeCustomer = await _stripeCustomerService.GetAsync(id);
        var customer = await _repository.GetByIdAsync(customerId);
        if (customer is null)
        {
            customer = _mapper.Map<Entities.Customer>(stripeCustomer);
        }
        
        await _repository.UpdateAsync(customer);
        return Results.NoContent();
    }
}