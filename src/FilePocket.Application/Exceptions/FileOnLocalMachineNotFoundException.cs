namespace FilePocket.Application.Exceptions;

public class FileOnLocalMachineNotFoundException : NotFoundException
{
    public FileOnLocalMachineNotFoundException(string filePath)
        : base($"There are no files on local machine, which are matched the requested path '{filePath}'.")
    { }
}
