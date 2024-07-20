using Contracts.Services;
using Shared.DTOs.Payment;
namespace Payment.API.Endpoints;

public static class PaymentEndpoints
{
    public static void MapPaymentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/payments")
            .WithTags("Payments");
        
        var checkoutSessionGroup = group.MapGroup("/checkout-sessions")
            .WithTags("Checkout Sessions");
        
        // POST create-checkout-session
        checkoutSessionGroup.MapPost("", async (CreatePaymentRequest request, IPaymentService paymentService) =>
        {
            // TODO: create a 'Payment' record in the database to store a summary of this payment.
            // TODO: set it's status to 'Pending' or 'Processing'.
            
            var response = await paymentService.CheckoutAsync(request);

            return Results.Ok(response);
        });
        
        // GET checkout-session/{sessionId}
        checkoutSessionGroup.MapGet("/{sessionId}", async (string sessionId, IPaymentService paymentService) =>
        {
            var status = await paymentService.GetCheckoutSessionStatusAsync(sessionId);

            return Results.Ok(status);
        });
    }
}