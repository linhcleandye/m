using Infrastructure.Exceptions;

namespace Customer.API.Exceptions;

public class NotFoundException(object id) : EntityNotFoundException("Customer", id);