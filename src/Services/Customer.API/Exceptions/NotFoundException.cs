using Infrastructure.Exceptions;

namespace Customer.API.Exceptions;

public class NotFoundException(int id) : EntityNotFoundException("Customer", id);