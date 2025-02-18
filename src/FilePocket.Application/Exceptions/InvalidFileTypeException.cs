namespace FilePocket.Application.Exceptions
{
    public class InvalidFileTypeException(string fileType)
        : ArgumentException($"Invalid file type '{fileType}' for image file")
    { }
}
