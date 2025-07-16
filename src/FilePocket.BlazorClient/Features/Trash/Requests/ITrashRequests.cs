using FilePocket.BlazorClient.Shared.Enums;

namespace FilePocket.BlazorClient.Features.Trash.Requests;

public interface ITrashRequests
{
    Task RestoreFromTrash(string itemType, string itemId);
    Task ClearAllTrashAsync();
    Task<List<T>> GetAllSoftdelted<T>(RequestedItemType itemType);
    Task<T?> GetSoftDeletedItem<T>(string itemType, string itemId);
}
