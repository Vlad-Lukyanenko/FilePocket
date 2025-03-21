using FilePocket.BlazorClient.Features.Storage.Models;
using FilePocket.BlazorClient.Services.Pockets;
using Newtonsoft.Json;

namespace FilePocket.BlazorClient.Features.Storage.Requests
{
    public class StorageRequests : IStorageRequests
    {
        private readonly FilePocketApiClient _apiClient;
        public StorageRequests(FilePocketApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<StorageConsumptionModel> GetStorageConsumption()
        {
            var content = await _apiClient.GetAsync(StorageUrl.GetStorageConsumption());

            return JsonConvert.DeserializeObject<StorageConsumptionModel>(content)!;

        }
    }
}
