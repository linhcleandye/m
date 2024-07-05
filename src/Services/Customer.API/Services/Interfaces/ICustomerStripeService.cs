namespace Customer.API.Services.Interfaces;

public interface ICustomerStripeService
{
    Task<IResult> GetByIdAsync(string id);
    Task<Stripe.Customer> GetCustomerByIdAsync(string id);
    Task<IResult> SyncByIdAsync(string id, int customerId);
}