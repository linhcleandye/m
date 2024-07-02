namespace Shared.DTOs.Payment;

public record CheckoutProductRequest
{
    public decimal Price { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }
}

public record CreatePaymentRequest
{
    public List<CheckoutProductRequest> Products { get; set; } = new();
    public int Quantity { get; set; }
    public Dictionary<string, string>? Metadata { get; set; }
    public string SuccessRedirectUrl { get; set; }
    public string CancelRedirectUrl { get; set; }
}