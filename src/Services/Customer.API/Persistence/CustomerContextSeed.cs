using Microsoft.EntityFrameworkCore;

namespace Customer.API.Persistence;

public static class CustomerContextSeed
{
    public static IHost SeedCustomerData(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var customerContext = scope.ServiceProvider
            .GetRequiredService<CustomerContext>();
        customerContext.Database.MigrateAsync().GetAwaiter().GetResult();

        CreateCustomer(customerContext, "customer1",
                "customer", "customer1@local.com")
            .GetAwaiter().GetResult();
        CreateCustomer(customerContext, "customer2", "customer2",
                "customer2@local.com")
            .GetAwaiter().GetResult();
        CreateCustomer(customerContext, "tedu", "Tedu", "tedu@yopmail.com")
            .GetAwaiter().GetResult();

        return host;
    }

    private static async Task CreateCustomer(CustomerContext customerContext, string firstName,
        string lastName, string email)
    {
        var customer = await customerContext.Customers
            .FirstOrDefaultAsync(x => x.EmailAddress.Equals(email));
        if (customer == null)
        {
            var newCustomer = new Entities.Customer
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email
            };
            newCustomer.UserName = email;
            await customerContext.Customers.AddAsync(newCustomer);
            await customerContext.SaveChangesAsync();
        }
    }
}