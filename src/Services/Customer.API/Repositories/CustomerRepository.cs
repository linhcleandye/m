using Contracts.Domains.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories;

public class CustomerRepository : RepositoryBase<Entities.Customer, int, CustomerContext>, ICustomerRepository
{
    public CustomerRepository(CustomerContext dbContext, IUnitOfWork<CustomerContext> unitOfWork) : base(dbContext, unitOfWork)
    {
    }

    public Task<Entities.Customer?> GetCustomerByUserNameOrEmailAsync(string username)
    {
        return FindByCondition(x => x.UserName.Equals(username) || x.EmailAddress.Equals(username))
            .SingleOrDefaultAsync();
    }

    public Task<Entities.Customer?> GetCustomerByEmailAsync(string email)
    {
        return FindByCondition(x => x.EmailAddress.Equals(email))
            .SingleOrDefaultAsync();
    }
}