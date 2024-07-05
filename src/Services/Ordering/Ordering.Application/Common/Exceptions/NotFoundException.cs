using Infrastructure.Exceptions;

namespace Ordering.Application.Common.Exceptions;

public class NotFoundException : EntityNotFoundException
{
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public NotFoundException(string name, object key)
        : base(name, key)
    {
    }
}