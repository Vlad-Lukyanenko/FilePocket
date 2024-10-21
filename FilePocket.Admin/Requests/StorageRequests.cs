using FilePocket.Admin.Models.Storage;
using FilePocket.Admin.Requests.Contracts;
using FilePocket.Admin.Requests.HttpRequests;
using Newtonsoft.Json;

namespace FilePocket.Admin.Requests;

public class StorageRequests : IStorageRequests
{
    private readonly IHttpRequests _authorizedRequests;

    public StorageRequests(IHttpRequests authorizedRequests)
    {
        _authorizedRequests = authorizedRequests;
    }


    public async Task<IEnumerable<StorageModel>> GetAllAsync()
    {
        var response = await _authorizedRequests.GetAsyncRequest("api/storages/all");
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<IEnumerable<StorageModel>>(content)!;
    }

    public async Task<StorageModel> GetDetails(Guid id)
    {
        var response = await _authorizedRequests.GetAsyncRequest($"api/storages/info/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<StorageModel>(content)!;
    }

    public async Task<bool> CheckStorageCapacity(Guid storageId, long newFileSize)
    {
        var url = $"api/storages/checksize/{storageId}?fileSize={newFileSize}";

        var response = await _authorizedRequests.GetAsyncRequest(url);

        var content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<bool>(content)!;
    }

    public async Task<bool> PostAsync(AddStorageModel storage)
    {
        var response = await _authorizedRequests.PostAsyncRequest("api/storages/", storage);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var response = await _authorizedRequests.DeleteAsyncRequest($"api/storages/{id}");

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PutAsync(StorageModel storage)
    {
        var response = await _authorizedRequests.PutAsyncRequest($"api/storages/{storage.Id}", storage);

        return response.IsSuccessStatusCode;
    }

}
