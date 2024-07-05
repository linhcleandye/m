namespace Infrastructure.Exceptions;

public class EntityNotFoundException : ApplicationException
{
    protected EntityNotFoundException(string entity, object key) :
        base($"Entity \"{entity}\" ({key}) was not found.")
    {
    }
}