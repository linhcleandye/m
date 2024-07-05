namespace Customer.API.Exceptions;

public class EmailExistedException(string email) : ApplicationException($"Customer with email: {email} already existed.")
{
}