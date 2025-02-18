using FilePocket.Application.Exceptions;

namespace FilePocket.Application.Exceptions;

public class PocketNotFoundException : NotFoundException
{
    public PocketNotFoundException(Guid pocketId)
        : base($"The pocket with id '{pocketId}' doesn't exist in the database.")
    {
    }
}
