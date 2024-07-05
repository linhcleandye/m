using Shared.DTOs.Customer;

namespace Customer.API.Services.Interfaces;

public interface ICustomerService
{
    Task<IResult> GetByUsernameAsync(string username);
    Task<IResult> GetAsync(int id);   
    Task<IResult> CreateAsync(CreateCustomerDto customerDto);   
    Task<IResult> UpdateAsync(int id, UpdateCustomerDto customerDto);   
    Task<IResult> DeleteAsync(int id);   
}