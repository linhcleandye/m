using Shared.DTOs.Customer.Stripe;

namespace Shared.DTOs.Payment;

public record CheckoutProductRequest
{
    public long Id { get; set; }
    public string No { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
    public string Summary { get; set; }
    public string ImageUrl { get; set; }
    public int Quantity { get; set; }
}

public record PaymentCustomerRequest
{
    private string? userName;
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Phone { get; set; }
    public StripeCustomerAddressDto? Address { get; set; }
    public StripeCustomerAddressDto? Shipping { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}

public record CreatePaymentRequest
{
    public required PaymentCustomerRequest Customer { get; set; }
    public List<CheckoutProductRequest> Products { get; set; } = new();
    public decimal Tax { get; set; }
    public decimal DeliveryFee { get; set; } 
    
    public decimal GetDeliveryFeePerItem() => DeliveryFee > 0 ? DeliveryFee / Products.Count : 0;
    public decimal GetTaxPerItem() => Tax > 0 ? Tax / Products.Count : 0;
    public Dictionary<string, string>? Metadata { get; set; }
    public string SuccessRedirectUrl { get; set; }
    public string CancelRedirectUrl { get; set; }
}