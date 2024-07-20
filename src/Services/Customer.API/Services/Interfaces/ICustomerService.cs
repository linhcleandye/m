using Shared.DTOs.Customer;

namespace Customer.API.Services.Interfaces;

public interface ICustomerService
{
    Task<IResult> GetByEmailAsync(string email);
    Task<IResult> GetAsync(int id);
    Task<IResult> CreateOrUpdateAsync(CreateCustomerDto customerDto);   
    Task<IResult> UpdateAsync(int id, UpdateCustomerDto customerDto);   
    Task<IResult> DeleteAsync(int id);   
}