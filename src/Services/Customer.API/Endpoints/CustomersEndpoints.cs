using Customer.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Customer;

namespace Customer.API.Controllers;

public static class CustomersEndpoints
{
    public static void MapCustomersEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
            .MapGroup("/api/customers")
            .HasApiVersion(1.0)
            .WithParameterValidation();

        group.MapGet("/{username}",
            async (string username, ICustomerService customerService) =>
                await customerService.GetCustomerByUsernameAsync(username));

        group.MapPost("/",
            async ([FromBody]CreateCustomerDto customerDto, ICustomerService customerService) =>
                await customerService.CreateCustomerAsync(customerDto));
        
        group.MapPut("/{id}",
            async (int id, [FromBody]UpdateCustomerDto customerDto, ICustomerService customerService) =>
                await customerService.UpdateCustomerAsync(id, customerDto));
    }
}