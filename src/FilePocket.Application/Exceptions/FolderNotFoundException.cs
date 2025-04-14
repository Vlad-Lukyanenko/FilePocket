namespace FilePocket.Application.Exceptions;

public class FolderNotFoundException : NotFoundException
{
    public FolderNotFoundException(Guid folderId)
        : base($"The folder with id '{folderId}' doesn't exist in the database.")
    {
    }
}
