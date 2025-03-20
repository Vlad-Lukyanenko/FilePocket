using FilePocket.BlazorClient.Features.Storage.Models;

namespace FilePocket.BlazorClient.Features.Storage.Requests
{
    public interface IStorageRequests
    {
        Task<StorageConsumptionModel> GetStorageConsumption();
    }
}
