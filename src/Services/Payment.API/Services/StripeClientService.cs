using Contracts.Services;
using Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Shared.DTOs.Payment;
using Stripe.Checkout;
using ILogger = Serilog.ILogger;
namespace Payment.API.Services;

public class StripeClientService : IPaymentService
{
    private readonly StripeConfig _stripeConfig;
    private readonly ILogger _logger;

    public StripeClientService(IOptions<StripeConfig> stripeConfig, ILogger logger)
    {
        _stripeConfig = stripeConfig.Value;
        _logger = logger;
    }

    public async Task<CreatePaymentResponse> Checkout(CreatePaymentRequest request)
    {
        var options = new SessionCreateOptions
        {
            Mode = "payment", // "setup" or "subscription"
            Currency = "aud", // "usd", "eur", "gbp", "aud", etc.
            PaymentMethodTypes =
            [
                "card",
            ],
            Metadata = request.Metadata,
            SuccessUrl = request.SuccessRedirectUrl,
            CancelUrl = request.CancelRedirectUrl,
            LineItems = request.Products.Select(lineItem => new SessionLineItemOptions
            {
                // Using Price and Product IDs on Stripe
                // Price = "{{PRICE_ID}}",
                // Product = "{{PRODUCT_ID}}",
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "aud",
                    UnitAmountDecimal = lineItem.Price * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = lineItem.Name,
                        Description = lineItem.Summary,
                        Images = [lineItem.ImageUrl],
                    },
                },
                Quantity = lineItem.Quantity
            }).ToList()
        };

        var checkoutSession = await new SessionService().CreateAsync(options);
        _logger.Information("Stripe Checkout Session created: {CheckoutSessionId}", checkoutSession.Id);
        return new CreatePaymentResponse(checkoutSession.Url, checkoutSession.Id);
    }
}