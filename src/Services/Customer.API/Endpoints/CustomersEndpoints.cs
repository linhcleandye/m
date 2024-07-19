using Customer.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Customer;

namespace Customer.API.Controllers;

internal static class CustomersEndpoints
{
    public static void MapCustomersEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
            .MapGroup("/api/customers")
            .HasApiVersion(1.0)
            .WithParameterValidation()
            .WithTags("Customers");

        group.MapGet("/username/{username}",
                async (string username, ICustomerService customerService) =>
                    await customerService.GetByEmailAsync(username));
        
        group.MapGet("/{id}",
                async (int id, ICustomerService customerService) =>
                    await customerService.GetAsync(id))
            .WithName(ApiEndpoints.Customers.GetCustomerById);

        group.MapPost("/",
            async ([FromBody] CreateCustomerDto customerDto, ICustomerService customerService) =>
            await customerService.CreateAsync(customerDto));

        group.MapPut("/{id}",
            async (int id, [FromBody] UpdateCustomerDto customerDto, ICustomerService customerService) =>
            await customerService.UpdateAsync(id, customerDto));

        group.MapDelete("/{id}",
            async (int id, ICustomerService customerService) =>
                await customerService.DeleteAsync(id));
    }
}