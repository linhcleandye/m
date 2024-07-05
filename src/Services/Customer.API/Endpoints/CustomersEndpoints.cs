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

        group.MapGet("/username/{username}",
            async (string username, ICustomerService customerService) =>
                await customerService.GetByUsernameAsync(username));
        
        group.MapGet("/{id}",
            async (int id, ICustomerService customerService) =>
                await customerService.GetAsync(id));

        group.MapPost("/",
            async ([FromBody]CreateCustomerDto customerDto, ICustomerService customerService) =>
                await customerService.CreateAsync(customerDto));
        
        group.MapPut("/{id}",
            async (int id, [FromBody]UpdateCustomerDto customerDto, ICustomerService customerService) =>
                await customerService.UpdateAsync(id, customerDto));

        group.MapDelete("/{id}",
            async (int id, ICustomerService customerService) =>
                await customerService.DeleteAsync(id));
    }
}