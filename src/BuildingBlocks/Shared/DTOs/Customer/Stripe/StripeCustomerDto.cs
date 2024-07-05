using System.Text.Json.Serialization;

namespace Shared.DTOs.Customer.Stripe;

public class StripeCustomerDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Currency { get; set; }
    public StripeCustomerAddressDto Address { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public string Phone { get; set; }
    public StripeCustomerAddressDto Shipping { get; set; }
}

public class StripeCustomerAddressDto
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Line1 { get; set; } = string.Empty;
    public string Line2 { get; set; } = string.Empty;
    [JsonPropertyName("postal_code")]
    public string PostalCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}