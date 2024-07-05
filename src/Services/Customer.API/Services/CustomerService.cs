using AutoMapper;
using Customer.API.Exceptions;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Shared.DTOs.Customer;
using Stripe;

namespace Customer.API.Services;

public class CustomerService(
    ICustomerRepository repository,
    IMapper mapper,
    IStripeCustomerRepository customerStripeRepository) : ICustomerService
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
        if (entity is null) throw new NotFoundException(id);

        if (!string.IsNullOrEmpty(entity.StripeCustomerId))
            entity.StripeCustomer = await customerStripeRepository.GetByIdAsync(entity.StripeCustomerId);

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
        entity.StripeCustomer.Shipping.Address = mapper.Map<Address>(customerDto.Shipping);
        entity.StripeCustomer.Phone = entity.StripeCustomer.Shipping.Phone = customerDto.Phone;
        repository.Create(entity);
        
        entity.StripeCustomer = await CreateStripeCustomerAsync(mapper.Map<CustomerDto>(entity));
        entity.StripeCustomerId = entity.StripeCustomer.Id;
        await repository.SaveChangesAsync();
        var result = mapper.Map<CustomerDto>(entity);
        return Results.Ok(result);
    }

    private async Task<Stripe.Customer> CreateStripeCustomerAsync(CustomerDto customerDto)
    {
        var addressOptions = GetAddressOptions(customerDto);
        var shippingOptions = GetShippingOptions(customerDto);

        var stripeCustomer = new CustomerCreateOptions
        {
            Email = customerDto.EmailAddress,
            Name = customerDto.FullName,
            Address = addressOptions,
            Shipping = shippingOptions,
        };

        return await customerStripeRepository.CreateAsync(stripeCustomer);
    }

    private AddressOptions GetAddressOptions(CustomerDto customerDto)
        => mapper.Map<AddressOptions>(customerDto.StripeCustomer.Address);

    private ShippingOptions GetShippingOptions(CustomerDto customerDto)
        => new()
        {
            Address = mapper.Map<AddressOptions>(customerDto.StripeCustomer.Shipping),
            Name = customerDto.FullName,
        };

    public async Task<IResult> UpdateAsync(int id, UpdateCustomerDto customerDto)
    {
        var existingCustomer = await repository.GetByIdAsync(id);
        if (existingCustomer is null)
            throw new NotFoundException(id);

        if (!string.IsNullOrEmpty(customerDto.StripeCustomerId))
            await UpdateStripeCustomerAsync(customerDto, existingCustomer);

        var entity = mapper.Map(customerDto, existingCustomer);
        await repository.UpdateAsync(entity);

        return Results.NoContent();
    }

    private async Task UpdateStripeCustomerAsync(UpdateCustomerDto customerDto, Entities.Customer existingCustomer)
    {
        existingCustomer.StripeCustomer = await customerStripeRepository.GetByIdAsync(customerDto.StripeCustomerId);
        existingCustomer.StripeCustomer = mapper.Map<Stripe.Customer>(customerDto);

        var shippingOptions = new ShippingOptions
        {
            Address = mapper.Map<AddressOptions>(customerDto.Shipping),
            Name = customerDto.FullName,
            Phone = customerDto.Phone
        };

        var updateOptions = new CustomerUpdateOptions
        {
            Address = mapper.Map<AddressOptions>(customerDto.Address),
            Shipping = shippingOptions,
            Phone = customerDto.Phone,
        };

        await customerStripeRepository.UpdateAsync(customerDto.StripeCustomerId, updateOptions);
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