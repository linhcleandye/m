namespace Payment.API.Exceptions;

public class CustomerIdNotValidException(string customerId) : ApplicationException($"Customer Id : {customerId} is not valid. It might not be created yet.");
