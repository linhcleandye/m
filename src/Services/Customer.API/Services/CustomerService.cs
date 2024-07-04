using AutoMapper;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Shared.DTOs.Customer;

namespace Customer.API.Services;

public class CustomerService : ICustomerService
{
    private readonly IMapper _mapper;
    private readonly ICustomerRepository _repository;

    public CustomerService(ICustomerRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IResult> GetCustomerByUsernameAsync(string username)
    {
        var entity = await _repository.GetCustomerByUserNameAsync(username);
        var result = _mapper.Map<CustomerDto>(entity);
        return result == null ? Results.NotFound() : Results.Ok(result);
    }

    public async Task<IResult> CreateCustomerAsync(CreateCustomerDto customerDto)
    {
        var entity = _mapper.Map<Entities.Customer>(customerDto);
        await _repository.CreateAsync(entity);
        var result = _mapper.Map<CustomerDto>(entity);
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateCustomerAsync(int id, UpdateCustomerDto customerDto)
    {
        var existingCustomer = await _repository.GetByIdAsync(id);
        if (existingCustomer is null)
            return Results.NotFound();
        
        var entity = _mapper.Map(customerDto, existingCustomer);
        await _repository.UpdateAsync(entity);
        return Results.NoContent();
    }
}