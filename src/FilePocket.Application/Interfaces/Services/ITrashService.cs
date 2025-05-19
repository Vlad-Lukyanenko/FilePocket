namespace FilePocket.Application.Interfaces.Services;

public interface ITrashService
{
    Task ClearAllTrashAsync(Guid userId);
}
