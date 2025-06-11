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

    public async Task MoveFileToTrash(Guid fileId)
    {
        var _ = await _apiClient.PutAsync(TrashUrl.MoveFileToTrash(fileId), null);
    }

    public async Task MovePocketToTrash(Guid pocketId)
    {
        var _ = await _apiClient.PutAsync(TrashUrl.MovePocketToTrash(pocketId), null);
    }

    public async Task ClearAllTrashAsync()
    {
        await _apiClient.DeleteAsync(TrashUrl.ClearAllTrash());
    }

    public async Task<List<T>> GetAllSoftdelted<T>(RequestedItemType itemType)
    {
        var url = TrashUrl.GetAllSoftdeleted(itemType);
        var content = await _apiClient.GetAsync(url);

        return JsonConvert.DeserializeObject<List<T>>(content)!;
    }
}
