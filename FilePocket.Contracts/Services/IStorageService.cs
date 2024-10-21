using FilePocket.Domain.Models;

namespace FilePocket.Contracts.Services;

public interface IStorageService
{
    Task<IEnumerable<StorageModel>> GetAllAsync(bool trackChanges);

    Task<StorageModel> GetByIdAsync(Guid storageId, bool trackChanges);

    Task<IEnumerable<StorageModel>> GetAllByUserIdAsync(Guid userId, bool trackChanges);

    Task<StorageModel> CreateStorageAsync(StorageForManipulationsModel storage);

    Task UpdateStorageAsync(Guid storageId, StorageForManipulationsModel storage, bool trackChanges);

    Task DeleteStorageAsync(Guid storageId, bool trackChanges);

    Task<StorageDetailsModel> GetStorageDetailsAsync(Guid storageId, bool trackChanges);

    Task<bool> GetComparingDefaultCapacityWithTotalFilesSizeInStorage(Guid storageId, double newFileSize);
}
