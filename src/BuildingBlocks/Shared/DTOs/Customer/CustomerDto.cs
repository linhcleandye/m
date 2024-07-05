using System.ComponentModel.DataAnnotations;
using Shared.DTOs.Customer.Stripe;

namespace Shared.DTOs.Customer;

public record CustomerDto(int Id, string UserName, string FirstName, string LastName, string EmailAddress)
{
    public StripeCustomerDto StripeCustomer { get; set; }
}

public record CreateCustomerDto(
    [StringLength(50)] string UserName,
    [Required] [StringLength(50)] string FirstName,
    [Required] [StringLength(150)] string LastName,
    [Required]
    [StringLength(250)]
    [EmailAddress]
    string EmailAddress,
    string? StripeCustomerId
)
{
    public string GetUserName() => string.IsNullOrWhiteSpace(UserName) ? EmailAddress : UserName;
}

public record UpdateCustomerDto(
    [Required] [StringLength(50)] string FirstName,
    [Required] [StringLength(150)] string LastName,
    string? StripeCustomerId
);