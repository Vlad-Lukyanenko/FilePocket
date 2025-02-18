namespace FilePocket.Application.Exceptions;

public class NotFoundException : Exception
{
    protected NotFoundException(string message)
        : base(message)
    { 
    }
}
