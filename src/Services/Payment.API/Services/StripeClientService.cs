using Contracts.Services;
using Payment.API.HttpRepositories.Interfaces;
using Shared.DTOs.Customer;
using Shared.DTOs.Payment;
using Stripe;
using Stripe.Checkout;
using ILogger = Serilog.ILogger;

namespace Payment.API.Services;

public class StripeClientService(ILogger logger, ICustomerRepository customerRepository) : IPaymentService
{
    public async Task<CreatePaymentResponse> Checkout(CreatePaymentRequest request)
    {
        var stripeCustomer = await GetOrCreateCustomer(request.Customer);
        
        var options = new SessionCreateOptions
        {
            Mode = "payment", // "setup" or "subscription"
            Currency = "aud", // "usd", "eur", "gbp", "aud", etc.
            PaymentMethodTypes =
            [
                "card",
            ],
            Metadata = request.Metadata,
            CustomerEmail = request.Customer!.Email,
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
        logger.Information("Stripe Checkout Session created: {CheckoutSessionId}", checkoutSession.Id);
        return new CreatePaymentResponse(checkoutSession.Url, checkoutSession.Id, stripeCustomer.Id);
    }

    private async Task<PaymentCustomerResponse> GetOrCreateCustomer(PaymentCustomerRequest request)
    {
        var existingCustomer = await customerRepository.GetByEmailAsync(request.Email);
        if (existingCustomer?.StripeCustomer != null && !string.IsNullOrEmpty(existingCustomer.StripeCustomer.Id))
            return new PaymentCustomerResponse(existingCustomer.StripeCustomer.Id);
        
        var options = new CustomerCreateOptions
        {
            Name = request.FullName,
            Email = request.Email,
            Address = new AddressOptions
            {
                City = request.Address?.City,
                Country = request.Address?.Country,
                Line1 = request.Address?.Street,
                PostalCode = request.Address?.Zip,
                State = request.Address?.State
            },
            Shipping = new ShippingOptions
            {
                Name = request.FullName,
                Address = new AddressOptions
                {
                    City = request.Shipping?.City,
                    Country = request.Shipping?.Country,
                    Line1 = request.Shipping?.Street,
                    PostalCode = request.Shipping?.Zip,
                    State = request.Shipping?.State
                },
                Phone = request.Phone
            },
            Phone = request.Phone
        };

        var service = new CustomerService();
        var customer = await service.CreateAsync(options);
        
        // Save the customer to the database of the Customer service
        var customerDto = new CreateCustomerDto(
            request.Email,
            request.FirstName,
            request.LastName,
            request.Phone,
            request.Address,
            request.Shipping,
            customer.Id
        );
        await customerRepository.CreateAsync(customerDto);
        
        return new PaymentCustomerResponse(customer.Id);
    }
}