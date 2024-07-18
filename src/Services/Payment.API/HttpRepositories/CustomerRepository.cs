using Infrastructure.Extensions;
using Payment.API.HttpRepositories.Interfaces;
using Shared.DTOs.Customer;

namespace Payment.API.HttpRepositories;

public class CustomerRepository(HttpClient client) : ICustomerRepository
{
    private const string Endpoint = "customers";

    public Task<CustomerDto?> GetByUserNameOrEmailAsync(string email) => client.GetFromJsonAsync<CustomerDto>($"{Endpoint}/username/{email}");

    /// <summary>
    /// Create a new customer (consumed from the Customer API)
    /// </summary>
    /// <param name="customerDto"></param>
    /// <returns></returns>
    public async Task<CustomerDto> CreateAsync(CreateCustomerDto customerDto)
    {
        var response = await client.PostAsJsonAsync($"{Endpoint}", customerDto);
        return await response.EnsureSuccessStatusCode().ReadContentAs<CustomerDto>();
    }
}