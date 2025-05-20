namespace FilePocket.BlazorClient.Features.Trash;

public interface ITrashRequests
{
    Task MoveFileToTrash(Guid fileId);
    Task MovePocketToTrash(Guid pocketId);
    Task ClearAllTrashAsync();
}
