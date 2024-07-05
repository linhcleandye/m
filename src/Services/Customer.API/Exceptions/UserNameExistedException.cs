namespace Customer.API.Exceptions;

public class UserNameExistedException(string username)
    : ApplicationException($"Customer with username : {username} already existed.");