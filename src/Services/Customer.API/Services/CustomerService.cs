using AutoMapper;
using Customer.API.Controllers;
using Customer.API.Exceptions;
using Customer.API.Repositories.Interfaces;
using Customer.API.Services.Interfaces;
using Shared.DTOs.Customer;
using Stripe;

namespace Customer.API.Services;

public class CustomerService(
    ICustomerRepository repository,
    IMapper mapper,
    IStripeCustomerRepository stripeCustomerRepository) : ICustomerService
{
    public async Task<IResult> GetByEmailAsync(string email)
    {
        var entity = await repository.GetByEmailAsync(email);
        if (entity is not null && !string.IsNullOrEmpty(entity.StripeCustomerId))
            entity.StripeCustomer = await stripeCustomerRepository.GetByIdAsync(entity.StripeCustomerId);

        var result = mapper.Map<CustomerDto>(entity);
        return result == null ? Results.NoContent() : Results.Ok(result);
    }

    public async Task<IResult> GetAsync(int id)
    {
        var entity = await repository.GetByIdAsync(id);
        if (entity is null) throw new NotFoundException(id);

        if (!string.IsNullOrEmpty(entity.StripeCustomerId))
            entity.StripeCustomer = await stripeCustomerRepository.GetByIdAsync(entity.StripeCustomerId);

        var result = mapper.Map<CustomerDto>(entity);
        return result == null ? throw new NotFoundException(id) : Results.Ok(result);
    }

    private async Task<CustomerDto?> GetCustomerByEmailAsync(string email)
    {
        var entity = await repository.GetByEmailAsync(email);
        return mapper.Map<CustomerDto>(entity);
    }

    public async Task<IResult> CreateOrUpdateAsync(CreateCustomerDto customerDto)
    {
        var customerByEmail = await GetCustomerByEmailAsync(customerDto.EmailAddress);
        var entity = mapper.Map<Entities.Customer>(customerDto);
        entity.Id = customerByEmail?.Id ?? 0;

        if (customerByEmail is null)
            await repository.CreateAsync(entity);

        await UpdateStripeCustomerAsync(customerDto, entity);
        await repository.SaveChangesAsync();
        
        var result = mapper.Map<CustomerDto>(entity);
        return Results.CreatedAtRoute(ApiEndpoints.Customers.GetCustomerById, new { id = entity.Id }, result);
    }

    private async Task UpdateStripeCustomerAsync(CreateCustomerDto customerDto, Entities.Customer entity)
    {
        entity.StripeCustomer = await GetOrCreateStripeCustomerAsync(entity, customerDto.StripeCustomerId);
        entity.StripeCustomerId = entity.StripeCustomer!.Id;
        entity.StripeCustomer.Shipping = new Shipping
        {
            Address = mapper.Map<Address>(customerDto.Shipping)
        };
        entity.StripeCustomer.Phone = entity.StripeCustomer.Shipping.Phone = customerDto.Phone;
    }

    private async Task<Stripe.Customer> GetOrCreateStripeCustomerAsync(
        Entities.Customer entity, string? stripeCustomerId)
    {
        if (string.IsNullOrEmpty(stripeCustomerId))
            return await CreateStripeCustomerAsync(mapper.Map<CustomerDto>(entity));

        var stripeCustomer = await stripeCustomerRepository.GetByIdAsync(stripeCustomerId);
        if (stripeCustomer?.Deleted != true)
            return stripeCustomer!;

        return await CreateStripeCustomerAsync(mapper.Map<CustomerDto>(entity));
    }

    private async Task<Stripe.Customer> CreateStripeCustomerAsync(CustomerDto customerDto)
    {
        var addressOptions = GetAddressOptions(customerDto);
        var shippingOptions = GetShippingOptions(customerDto);
        var metadata = new Dictionary<string, string>
        {
            { "customer_id", customerDto.Id.ToString() }
        };

        var stripeCustomer = new CustomerCreateOptions
        {
            Email = customerDto.EmailAddress,
            Name = customerDto.FullName(),
            Address = addressOptions,
            Shipping = shippingOptions,
            Metadata = metadata
        };

        return await stripeCustomerRepository.CreateAsync(stripeCustomer);
    }

    private AddressOptions GetAddressOptions(CustomerDto customerDto)
        => mapper.Map<AddressOptions>(customerDto.StripeCustomer.Address);

    private ShippingOptions GetShippingOptions(CustomerDto customerDto)
        => new()
        {
            Address = mapper.Map<AddressOptions>(customerDto.StripeCustomer.Shipping),
            Name = customerDto.FullName()
        };

    public async Task<IResult> UpdateAsync(int id, UpdateCustomerDto customerDto)
    {
        var existingCustomer = await repository.GetByIdAsync(id);
        if (existingCustomer is null)
            throw new NotFoundException(id);

        var entity = mapper.Map(customerDto, existingCustomer);
        if (!string.IsNullOrEmpty(customerDto.StripeCustomerId))
        {
            var stripeCustomer = await UpdateStripeCustomerAsync(customerDto, existingCustomer);
            entity.StripeCustomer = stripeCustomer;
        }

        await repository.UpdateAsync(entity);
        var result = mapper.Map<CustomerDto>(entity);
        return Results.AcceptedAtRoute(ApiEndpoints.Customers.GetCustomerById, new { id = entity.Id }, result);
    }

    private async Task<Stripe.Customer> UpdateStripeCustomerAsync(UpdateCustomerDto customerDto,
        Entities.Customer existingCustomer)
    {
        existingCustomer.StripeCustomer = await stripeCustomerRepository.GetByIdAsync(customerDto.StripeCustomerId);
        var metadata = new Dictionary<string, string>
        {
            { "customer_id", existingCustomer.Id.ToString() }
        };
        var shippingOptions = new ShippingOptions
        {
            Address = mapper.Map<AddressOptions>(customerDto.Shipping),
            Name = customerDto.FullName,
            Phone = customerDto.Phone
        };

        var updateOptions = new CustomerUpdateOptions
        {
            Name = customerDto.FullName,
            Address = mapper.Map<AddressOptions>(customerDto.Address),
            Shipping = shippingOptions,
            Phone = customerDto.Phone,
            Metadata = metadata,
        };

        return await stripeCustomerRepository.UpdateAsync(customerDto.StripeCustomerId, updateOptions);
    }

    public async Task<IResult> DeleteAsync(int id)
    {
        var existingCustomer = await repository.GetByIdAsync(id);
        if (existingCustomer is null)
            throw new NotFoundException(id);

        if (!string.IsNullOrEmpty(existingCustomer.StripeCustomerId))
            await stripeCustomerRepository.DeleteAsync(existingCustomer.StripeCustomerId, null);

        await repository.DeleteAsync(existingCustomer);
        return Results.NoContent();
    }
}