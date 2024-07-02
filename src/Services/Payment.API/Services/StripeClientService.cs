using Infrastructure.Configurations;
using Microsoft.Extensions.Options;
using Shared.DTOs.Payment;
using Stripe;
using Stripe.Checkout;
using ILogger = Serilog.ILogger;
namespace Payment.API.Services;

public class StripeClientService
{
    private readonly StripeConfig _stripeConfig;
    private readonly ILogger _logger;

    public StripeClientService(IOptions<StripeConfig> stripeConfig, ILogger logger)
    {
        _stripeConfig = stripeConfig.Value;
        _logger = logger;
        
        StripeConfiguration.ApiKey = _stripeConfig.ApiKey; // must set the API key
    }

    public async Task<string> Checkout(CreatePaymentRequest request)
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
            LineItems = request.Products.Select(product => new SessionLineItemOptions
            {
                // Using Price and Product IDs on Stripe
                // Price = "{{PRICE_ID}}",
                // Product = "{{PRODUCT_ID}}",
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "aud",
                    UnitAmountDecimal = product.Price * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = product.Name,
                        Description = product.Description,
                        Images = [product.ImageUrl],
                    },
                },
                Quantity = request.Quantity,
            }).ToList()
        };

        var checkoutSession = await new SessionService().CreateAsync(options);
        return checkoutSession.Url;
    }
}