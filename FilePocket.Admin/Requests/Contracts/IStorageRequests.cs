using FilePocket.Admin.Models.Storage;

namespace FilePocket.Admin.Requests.Contracts;

public interface IStorageRequests
{
    Task<IEnumerable<StorageModel>> GetAllAsync();
    Task<bool> PostAsync(AddStorageModel storage);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> PutAsync(StorageModel storage);
    Task<StorageModel> GetDetails(Guid id);
    Task<bool> CheckStorageCapacity(Guid storageId, long newFileSize);
}