namespace FilePocket.Shared.Exceptions;

public class StorageNotFoundException : NotFoundException
{
    public StorageNotFoundException(Guid storageId)
        : base($"The storage with id '{storageId}' doesn't exist in the database.")
    {
    }
}
