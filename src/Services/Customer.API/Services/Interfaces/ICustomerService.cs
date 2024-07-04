using Shared.DTOs.Customer;

namespace Customer.API.Services.Interfaces;

public interface ICustomerService
{
    Task<IResult> GetCustomerByUsernameAsync(string username);
    Task<IResult> CreateCustomerAsync(CreateCustomerDto customerDto);   
    Task<IResult> UpdateCustomerAsync(int id, UpdateCustomerDto customerDto);   
}