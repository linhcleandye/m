using AutoMapper;
using Customer.API.Services.Interfaces;
using Shared.DTOs.Customer;

namespace Customer.API.Services;

public class CustomerStripeService : Stripe.CustomerService, ICustomerStripeService
{
    private readonly Stripe.CustomerService _service;
    private readonly IMapper _mapper;

    public CustomerStripeService(IMapper mapper, Stripe.CustomerService service)
    {
        _mapper = mapper;
        _service = service;
    }
    
    public async Task<IResult> GetByIdAsync(string id)
    {
        var stripeCustomer = await _service.GetAsync(id);
        var result = _mapper.Map<StripeCustomerDto>(stripeCustomer);

        return Results.Ok(result);
    }
}