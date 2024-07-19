using Shared.DTOs.Customer;

namespace Payment.API.HttpRepositories.Interfaces;

public interface ICustomerRepository
{
    Task<CustomerDto?> GetByEmailAsync(string email);
    Task<CustomerDto?> CreateAsync(CreateCustomerDto customerDto);
}