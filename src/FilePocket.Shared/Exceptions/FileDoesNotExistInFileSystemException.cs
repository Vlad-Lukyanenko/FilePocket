namespace FilePocket.Shared.Exceptions;

/// <summary>
/// It might be okay to throw this exception if the file doesn't exist in the file system.
/// But the assumption is that the file should not be removed outside the application.
/// </summary>
public class FileDoesNotExistInFileSystemException(Guid fileId)
    : NotFoundException($"The File with id '{fileId}' doesn't exist in the file system.");