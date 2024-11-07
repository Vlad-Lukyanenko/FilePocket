using FilePocket.Domain.Models;

namespace FilePocket.Contracts.Services;

public interface IPocketService
{
    Task<StorageModel> GetByIdAsync(Guid storageId, bool trackChanges);

    Task<IEnumerable<StorageModel>> GetAllByUserIdAsync(Guid userId, bool trackChanges);

    Task<StorageModel> CreateStorageAsync(StorageForManipulationsModel storage);

    Task UpdateStorageAsync(Guid pocketId, StorageForManipulationsModel storage, bool trackChanges);

    Task DeleteStorageAsync(Guid pocketId, bool trackChanges);

    Task<StorageDetailsModel> GetStorageDetailsAsync(Guid pocketId, bool trackChanges);

    Task<bool> GetComparingDefaultCapacityWithTotalFilesSizeInStorage(Guid pocketId, double newFileSize);
}
