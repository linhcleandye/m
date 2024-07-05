namespace Customer.API.Services.Interfaces;

public interface ICustomerStripeService
{
    Task<IResult> GetByIdAsync(string id);
}