namespace Customer.API.Controllers;

public static class ApiEndpoints
{
    public static class Customers
    {
        private const string Base = "api/customers";
        public const string GetCustomerById = $"{Base}/{{id:int}}";
    }
}