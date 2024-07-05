using Customer.API.Services.Interfaces;

namespace Customer.API.Controllers;

public static class CustomersStripeEndpoints
{
    public static void MapCustomersStripeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
            .MapGroup("/api/stripe/customers")
            .HasApiVersion(1.0)
            .WithParameterValidation();

        group.MapGet("/{id}",
            async (string id,
                    ICustomerStripeService customerService) =>
                await customerService.GetByIdAsync(id));
    }
}