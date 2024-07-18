using Contracts.Domains.Interfaces;
using Customer.API.Persistence;

namespace Customer.API.Repositories.Interfaces;

public interface ICustomerRepository : IRepositoryBase<Entities.Customer, int, CustomerContext>
{
    Task<Entities.Customer?> GetCustomerByUserNameOrEmailAsync(string username);
    Task<Entities.Customer?> GetCustomerByEmailAsync(string email);
}