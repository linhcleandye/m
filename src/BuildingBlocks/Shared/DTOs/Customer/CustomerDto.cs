using System.ComponentModel.DataAnnotations;
using Shared.DTOs.Customer.Stripe;

namespace Shared.DTOs.Customer;

public record CustomerDto(int Id, string UserName, string FirstName, string LastName, string EmailAddress, StripeCustomerDto StripeCustomer)
{
    public string FullName() => $"{FirstName} {LastName}";
}

public record CreateCustomerDto(
    [StringLength(50)] string? UserName,
    [Required] [StringLength(50)] string FirstName,
    [Required] [StringLength(150)] string LastName,
    [Required]
    [StringLength(250)]
    [EmailAddress]
    string EmailAddress,
    string? Phone,
    StripeCustomerAddressDto? Address,
    StripeCustomerAddressDto? Shipping
)
{
    public string GetUserName() => string.IsNullOrWhiteSpace(UserName) ? EmailAddress : UserName;
    public string FullName() => $"{FirstName} {LastName}";
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
    public string FullName() => $"{FirstName} {LastName}";
}