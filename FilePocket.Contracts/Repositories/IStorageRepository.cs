using FilePocket.Domain.Entities;

namespace FilePocket.Contracts.Repositories;

public interface IStorageRepository
{
    Task<IEnumerable<Storage>> GetAllAsync(Guid userId,bool trackChanges);

    Task<Storage> GetByIdAsync(Guid storageId, bool trackChanges = false);

    Task<IEnumerable<Storage>> GetAllByUserIdAsync(Guid userId, bool trackChanges = false);

    void CreateStorage(Storage storage);

    void DeleteStorage(Storage storage);

    Task<(string Name, DateTime DateCreated, int NumberOfFiles, double TotalFileSize)> GetStorageDetailsAsync(Guid storageId, bool trackChanges);
    
    Task<double> GetTotalFileSizeAsync(Guid storageId, bool trackChanges);
}