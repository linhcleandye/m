using AutoMapper;
using Customer.API.Exceptions;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Shared.DTOs.Customer;

namespace Customer.API.Services;

public class CustomerService(ICustomerRepository repository, IMapper mapper) : ICustomerService
{
    public async Task<IResult> GetByUsernameAsync(string username)
    {
        var entity = await repository.GetCustomerByUserNameAsync(username);
        var result = mapper.Map<CustomerDto>(entity);
        return result == null ? Results.NotFound() : Results.Ok(result);
    }

    public async Task<IResult> GetAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        var result = mapper.Map<CustomerDto>(entity);
        return result == null ? throw new NotFoundException(id) : Results.Ok(result);
    }
    
    private async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
    {
        var entity = await repository.GetCustomerByEmailAsync(email);
        return mapper.Map<CustomerDto>(entity);
    }
    
    private async Task<CustomerDto?> GetCustomerByUsernameAsync(string username)
    {
        var entity = await repository.GetCustomerByUserNameAsync(username);
        return mapper.Map<CustomerDto>(entity);
    }

    public async Task<IResult> CreateAsync(CreateCustomerDto customerDto)
    {
        if (customerDto.GetUserName() != customerDto.EmailAddress)
        {
            var customerByUserName = await GetCustomerByUsernameAsync(customerDto.GetUserName());
            if (customerByUserName is not null)
                throw new UserNameExistedException(customerDto.GetUserName());
        }

        var customerByEmail = await GetCustomerByEmailAsync(customerDto.EmailAddress);
        if (customerByEmail is not null)
            throw new EmailExistedException(customerDto.EmailAddress);
        
        var entity = mapper.Map<Entities.Customer>(customerDto);
        await repository.CreateAsync(entity);
        var result = mapper.Map<CustomerDto>(entity);
        return Results.Ok(result);
    }
    
    public async Task<IResult> UpdateAsync(int id, UpdateCustomerDto customerDto)
    {
        var existingCustomer = await repository.GetByIdAsync(id);
        if (existingCustomer is null)
            throw new NotFoundException(id);
        
        var entity = mapper.Map(customerDto, existingCustomer);
        await repository.UpdateAsync(entity);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var existingCustomer = await repository.GetByIdAsync(id);
        if (existingCustomer is null)
            throw new NotFoundException(id);

        await repository.DeleteAsync(existingCustomer);
        return Results.NoContent();
    }
}