using FilePocket.BlazorClient.Features.Search.Enums;
using Newtonsoft.Json;

namespace FilePocket.BlazorClient.Features.Trash.Requests;

public class TrashRequests : ITrashRequests
{
    private readonly FilePocketApiClient _apiClient;

    public TrashRequests(FilePocketApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task RestoreFromTrash(string itemType, string itemId)
    {
        var url = TrashUrl.RestoreFromTrash(itemType, itemId);
        var response = await _apiClient.PutAsync(url, null);

        if (response.StatusCode != System.Net.HttpStatusCode.OK)
        {
            throw new Exception($"Failed to restore item from trash. Status code: {response.StatusCode}");
        }
    }

    public async Task ClearAllTrashAsync()
    {
        await _apiClient.DeleteAsync(TrashUrl.ClearAllTrash());
    }

    public async Task<List<T>> GetAllSoftdelted<T>(RequestedItemType itemType)
    {
        var url = TrashUrl.GetAllDeletedItems(itemType);
        var content = await _apiClient.GetAsync(url);

        return JsonConvert.DeserializeObject<List<T>>(content)!;
    }

    public async Task<T?> GetSoftDeletedItem<T>(string itemType, string itemId)
    {
        var url = TrashUrl.GetDeletedItem(itemType, itemId);
        var response = await _apiClient.GetAsync(url) ?? string.Empty;

        return JsonConvert.DeserializeObject<T>(response);
    }


}
