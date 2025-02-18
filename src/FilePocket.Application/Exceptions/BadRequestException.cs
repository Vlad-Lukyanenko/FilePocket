namespace FilePocket.Application.Exceptions;

public class BadRequestException : Exception
{
    protected BadRequestException(string message)
        : base(message)
    {
    }
}
