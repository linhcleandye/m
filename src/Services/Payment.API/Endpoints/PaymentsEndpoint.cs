using Payment.API.Services;
using Shared.DTOs.Payment;
namespace Payment.API.Endpoints;

public static class PaymentEndpoints
{
    public static RouteGroupBuilder MapPaymentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/payments")
            .WithTags("Payments");
        
        // GET ENDPOINTS
        group.MapPost("/create-checkout-session", async (CreatePaymentRequest request, StripeClientService stripeClient) =>
        {
            // TODO: create a 'Payment' record in the database to store a summary of this payment.
            // TODO: set it's status to 'Pending' or 'Processing'.
            
            var checkoutUrl = await stripeClient.Checkout(request);

            return Results.Ok(new CreatePaymentResponse(checkoutUrl));
        });

        return group;
    }
}