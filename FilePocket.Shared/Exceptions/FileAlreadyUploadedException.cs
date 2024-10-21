namespace FilePocket.Shared.Exceptions;

public class FileAlreadyUploadedException : Exception
{
    public FileAlreadyUploadedException(string fileName)
        : base($"File with name '{fileName}' has been already uploaded. Change the file name.")
    {
    }
}
