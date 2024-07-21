using Contracts.Services;
using Payment.API.HttpRepositories.Interfaces;
using Shared.DTOs.Customer;
using Shared.DTOs.Payment;
using Stripe;
using Stripe.Checkout;
using ILogger = Serilog.ILogger;

namespace Payment.API.Services;

public class StripePaymentService(ILogger logger, ICustomerRepository customerRepository) : IPaymentService
{
    public async Task<CreatePaymentResponse> CheckoutAsync(CreatePaymentRequest request)
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

    public async Task<PaymentResponse> GetCheckoutSessionStatusAsync(string sessionId)
    {
        var options = new SessionGetOptions();
        var session = await new SessionService().GetAsync(sessionId, options);
        return new PaymentResponse(session.Status, session.PaymentStatus, session.AmountTotal, session.CustomerEmail);
    }

    private async Task<PaymentCustomerResponse> GetOrCreateCustomer(PaymentCustomerRequest request)
    {
        var paymentCustomerResponse = await TryGetCustomerByEmail(request.Email);
        if (!string.IsNullOrEmpty(paymentCustomerResponse.Id))
            return new PaymentCustomerResponse(paymentCustomerResponse.Id);
        
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
        await TryCreateOrUpdateCustomerAsync(request, customer.Id);
        
        return new PaymentCustomerResponse(customer.Id);
    }
    
    // Get the customer from the database of the Customer service
    private async Task<GetCustomerResponse> TryGetCustomerByEmail(string email)
    {
        try
        {
            var existingCustomer = await customerRepository.GetByEmailAsync(email);
            
            if (!string.IsNullOrEmpty(existingCustomer?.StripeCustomerId) && existingCustomer.StripeCustomer.Deleted is false)
            {
                var stripeCustomer = await new CustomerService().GetAsync(existingCustomer.StripeCustomer.Id);
                if (stripeCustomer is not null)
                    return new GetCustomerResponse(existingCustomer.StripeCustomer.Id);
            }
        }
        catch (Exception e)
        {
            logger.Error(e.Message);
        }
        return new GetCustomerResponseFailed("Customer not found | Customer service error");
    }
    
    // Save the customer to the database of the Customer service
    private async Task TryCreateOrUpdateCustomerAsync(PaymentCustomerRequest request, string stripeCustomerId)
    {
        try
        {
            var customerDto = new CreateCustomerDto(
                request.Email,
                request.FirstName,
                request.LastName,
                request.Phone,
                request.Address,
                request.Shipping,
                stripeCustomerId
            );
            await customerRepository.CreateOrUpdateAsync(customerDto);
        }
        catch (Exception e)
        {
            logger.Error(e.Message);
        }
    }
}