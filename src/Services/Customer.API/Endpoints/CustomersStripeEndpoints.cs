using System.ComponentModel.DataAnnotations;
using Customer.API.Services.Interfaces;
namespace Customer.API.Controllers;

public static class CustomersStripeEndpoints
{
    public static void MapCustomersStripeEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
            .MapGroup("/api/stripe/customers")
            .HasApiVersion(1.0)
            .WithParameterValidation()
            .WithTags("StripeCustomers");

        group.MapGet("/{id}",
            async (string id,
                    IStripeCustomerService customerService) =>
                await customerService.GetByIdAsync(id))
            .WithName("GetStripeCustomerById");
        
        group.MapPatch("/sync-customers/{id}/customers/{customerId:int}",
            async ([Required]string id, [Required]int customerId,
                    IStripeCustomerService customerService) =>
                await customerService.SyncByIdAsync(id, customerId));
        
    }
}