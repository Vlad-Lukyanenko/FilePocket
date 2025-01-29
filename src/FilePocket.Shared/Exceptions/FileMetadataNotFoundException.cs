namespace FilePocket.Shared.Exceptions;

public class FileMetadataNotFoundException : NotFoundException
{
    public FileMetadataNotFoundException(Guid fileMetadataId)
        : base($"The FileMetadata with id '{fileMetadataId}' doesn't exist in the database.")
    {
    }
}
