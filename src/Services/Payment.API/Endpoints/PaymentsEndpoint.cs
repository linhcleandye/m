using Contracts.Services;
using Shared.DTOs.Payment;
namespace Payment.API.Endpoints;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/payments")
            .WithTags("Payments");
        
        // GET ENDPOINTS
        group.MapPost("/create-checkout-session", async (CreatePaymentRequest request, IPaymentService paymentService) =>
        {
            // TODO: create a 'Payment' record in the database to store a summary of this payment.
            // TODO: set it's status to 'Pending' or 'Processing'.
            if (request.Customer is not null)
            {
                var customerResponse = await paymentService.CreateCustomer(request.Customer);
                request.Customer.CustomerId = customerResponse.Id;
            }

            var response = await paymentService.Checkout(request);

            return Results.Ok(response);
        });

        return group;
    }
}