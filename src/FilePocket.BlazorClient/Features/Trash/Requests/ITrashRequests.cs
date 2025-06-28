using FilePocket.BlazorClient.Features.Search.Enums;
using FilePocket.BlazorClient.Features.Trash.Models;

namespace FilePocket.BlazorClient.Features.Trash.Requests;

public interface ITrashRequests
{
    Task RestoreFromTrash(string itemType, string itemId);
    Task ClearAllTrashAsync();
    Task<List<T>> GetAllSoftdelted<T>(RequestedItemType itemType);
    Task<T?> GetSoftDeletedItem<T>(string itemType, string itemId);
}
