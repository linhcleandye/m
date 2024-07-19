using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Contracts.Domains;

namespace Customer.API.Entities;

public class Customer : EntityBase<int>
{
    private string userName;
    [Required] public string UserName { get => EmailAddress; set => userName = value; }

    [Required]
    [Column(TypeName = "varchar(100)")]
    public string FirstName { get; set; }

    [Required]
    [Column(TypeName = "varchar(150)")]
    public string LastName { get; set; }

    [Required] [EmailAddress] public string EmailAddress { get; set; }
    
    public string? StripeCustomerId { get; set; }

    [NotMapped] public Stripe.Customer StripeCustomer { get; set; }
}