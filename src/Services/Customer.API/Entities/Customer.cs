using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;
using Newtonsoft.Json;

namespace Customer.API.Entities;

public class Customer : EntityBase<int>
{
    [Required] public string UserName { get; set; }

    [Required]
    [Column(TypeName = "varchar(100)")]
    public string FirstName { get; set; }

    [Required]
    [Column(TypeName = "varchar(150)")]
    public string LastName { get; set; }

    [Required] [EmailAddress] public string EmailAddress { get; set; }
    
    // [Column(TypeName = "jsonb")]
    // public StripeCustomer? StripeCustomer { get; set; }
}

[NotMapped]
public class StripeCustomer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Currency { get; set; }
    public StripeCustomerAddress Address { get; set; }
    public Dictionary<string, string> Metadata { get; set; }
    public string Phone { get; set; }
    public StripeCustomerAddress Shipping { get; set; }
}

[NotMapped]
public class StripeCustomerAddress
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Line1 { get; set; } = string.Empty;
    public string Line2 { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
}