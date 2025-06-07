using FilePocket.BlazorClient.Features.Search.Enums;

namespace FilePocket.BlazorClient.Features.Trash.Requests;

public interface ITrashRequests
{
    Task MoveFileToTrash(Guid fileId);
    Task MovePocketToTrash(Guid pocketId);
    Task ClearAllTrashAsync();
    Task<List<T>> GetAllSoftdelted<T>(RequestedItemType itemType);


}
