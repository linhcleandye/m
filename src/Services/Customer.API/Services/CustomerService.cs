using AutoMapper;
using Customer.API.Exceptions;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Infrastructure.Exceptions;
using Shared.DTOs.Customer;

namespace Customer.API.Services;

public class CustomerService(ICustomerRepository repository, IMapper mapper) : ICustomerService
{
    public async Task<IResult> GetCustomerByUsernameAsync(string username)
    {
        var entity = await repository.GetCustomerByUserNameAsync(username);
        var result = mapper.Map<CustomerDto>(entity);
        return result == null ? Results.NotFound() : Results.Ok(result);
    }

    public async Task<IResult> GetCustomerAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        var result = mapper.Map<CustomerDto>(entity);
        return result == null ? throw new NotFoundException(id) : Results.Ok(result);
    }

    public async Task<IResult> CreateCustomerAsync(CreateCustomerDto customerDto)
    {
        var entity = mapper.Map<Entities.Customer>(customerDto);
        await repository.CreateAsync(entity);
        var result = mapper.Map<CustomerDto>(entity);
        return Results.Ok(result);
    }

    public async Task<IResult> UpdateCustomerAsync(int id, UpdateCustomerDto customerDto)
    {
        var existingCustomer = await repository.GetByIdAsync(id);
        if (existingCustomer is null)
            throw new NotFoundException(id);
        
        var entity = mapper.Map(customerDto, existingCustomer);
        await repository.UpdateAsync(entity);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteCustomerAsync(int id)
    {
        var existingCustomer = await repository.GetByIdAsync(id);
        if (existingCustomer is null)
            throw new NotFoundException(id);

        await repository.DeleteAsync(existingCustomer);
        return Results.NoContent();
    }
}