using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs.Customer;

public record CustomerDto
{
    public string UserName { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string EmailAddress { get; set; }
}

public record CreateCustomerDto(
    [Required] [StringLength(50)] string UserName,
    [Required] [StringLength(50)] string FirstName,
    [Required] [StringLength(150)] string LastName,
    [Required]
    [StringLength(250)]
    [EmailAddress]
    string EmailAddress
);