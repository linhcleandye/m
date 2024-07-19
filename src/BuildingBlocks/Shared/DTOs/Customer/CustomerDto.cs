using System.ComponentModel.DataAnnotations;
using Shared.DTOs.Customer.Stripe;

namespace Shared.DTOs.Customer;

public record CustomerDto(int Id, string UserName, string FirstName, string LastName, string EmailAddress, StripeCustomerDto StripeCustomer)
{
    public string FullName() => $"{FirstName} {LastName}";
    public string? StripeCustomerId { get; set; }
}

public record CreateCustomerDto(
    [Required]
    [EmailAddress]
    string EmailAddress,
    [Required] [StringLength(50)] string FirstName,
    [Required] [StringLength(150)] string LastName,
    [Required]
    [StringLength(250)]
    string? Phone,
    StripeCustomerAddressDto? Address,
    StripeCustomerAddressDto? Shipping,
    string? StripeCustomerId
)
{
    public string FullName() => $"{FirstName} {LastName}";
    public string UserName => EmailAddress;
}

public record UpdateCustomerDto(
    [Required] [StringLength(50)] string FirstName,
    [Required] [StringLength(150)] string LastName,
    [Required] string StripeCustomerId,
    string? Phone,
    StripeCustomerAddressDto? Address,
    StripeCustomerAddressDto? Shipping
)
{
    public string FullName => $"{FirstName} {LastName}";
}