using System.Net;
using Contracts.Services;
using Payment.API.Exceptions;
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
        return new CreatePaymentResponse(checkoutSession.Url, checkoutSession.Id, request.Customer?.CustomerId);
    }

    public async Task<PaymentCustomerResponse> CreateCustomer(PaymentCustomerRequest request)
    {
        request = await GetOrCreateCustomer(request);
        
        var options = new CustomerCreateOptions
        {
            Name = request.FullName,
            Email = request.Email,
            Address = new AddressOptions
            {
                City = request.Address?.City,
                Country = request.Address?.Country,
                Line1 = request.Address?.Line1,
                Line2 = request.Address?.Line2,
                PostalCode = request.Address?.PostalCode,
                State = request.Address?.State
            },
            Shipping = new ShippingOptions
            {
                Name = request.FullName,
                Address = new AddressOptions
                {
                    City = request.Shipping?.City,
                    Country = request.Shipping?.Country,
                    Line1 = request.Shipping?.Line1,
                    Line2 = request.Shipping?.Line2,
                    PostalCode = request.Shipping?.PostalCode,
                    State = request.Shipping?.State
                },
                Phone = request.Phone
            },
            Phone = request.Phone
        };

        var service = new CustomerService();
        var customer = await service.CreateAsync(options);
        return new PaymentCustomerResponse(customer.Id);
    }

    private async Task<PaymentCustomerRequest> GetOrCreateCustomer(PaymentCustomerRequest request)
    {
        try
        {
            var existingCustomer = await customerRepository.GetByUserNameOrEmailAsync(request.Email);
            if (existingCustomer != null)
            {
                if (!string.IsNullOrEmpty(request.CustomerId) && existingCustomer.StripeCustomer.Id != request.CustomerId)
                    throw new CustomerIdNotValidException(existingCustomer.StripeCustomer.Id);
                
                request.CustomerId = existingCustomer.StripeCustomer.Id;
            }
        }
        catch (HttpRequestException e)
        {
            switch (e.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    var createCustomerDto = new CreateCustomerDto(request.UserName, request.FirstName, request.LastName,
                        request.Email, request.Phone, request.Address, request.Shipping);
                    var customerCreated = await customerRepository.CreateAsync(createCustomerDto);
                    request.CustomerId = customerCreated.StripeCustomer.Id;
                    break;
            }
        }
        catch (Exception e)
        {
            logger.Error(e, "Error occurred while creating customer");
            throw new Exception("Error occurred while creating customer");
        }

        return request;
    }
}