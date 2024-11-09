using FilePocket.Contracts.Repositories;
using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.DataAccess.Repositories;

public class StorageRepository : RepositoryBase<Storage>, IStorageRepository
{
    public StorageRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public async Task<IEnumerable<Storage>> GetAllAsync(Guid userId, bool trackChanges)
    {
        return await FindByCondition(c => c.UserId == userId, trackChanges).OrderByDescending(c => c.DateCreated).ToListAsync();
    }

    public async Task<IEnumerable<Storage>> GetAllByUserIdAsync(Guid userId, bool trackChanges)
    {
        return await FindByCondition(e => e.UserId.Equals(userId), trackChanges).ToListAsync();
    }

    public async Task<Storage> GetByIdAsync(Guid storageId, bool trackChanges)
    {
        return (await FindByCondition(c => c.Id.Equals(storageId), trackChanges).SingleOrDefaultAsync())!;
    }

    public async Task<(string Name, DateTime DateCreated, int NumberOfFiles, double TotalFileSize)> GetStorageDetailsAsync(Guid storageId, bool trackChanges)
    {
        var query = FindByCondition(c => c.Id.Equals(storageId), trackChanges);

        var storage = await query
            .Include(s => s.FileUploadSummaries)
            .SingleOrDefaultAsync();

        var totalFileSize = storage.FileUploadSummaries?.Sum(f => f.FileSize) ?? 0;
        var numberOfFiles = storage.FileUploadSummaries?.Count ?? 0;

        return (storage.Name, storage.DateCreated, numberOfFiles, totalFileSize);
    }

    public async Task<double> GetTotalFileSizeAsync(Guid storageId, bool trackChanges)
    {
        var query = FindByCondition(c => c.Id.Equals(storageId), trackChanges);

        var storage = await query
            .Include(s => s.FileUploadSummaries)
            .FirstOrDefaultAsync(s => s.Id == storageId);

        var totalFileSizeInKilobytes = storage.FileUploadSummaries?.Sum(f => f.FileSize) ?? 0;
        var totalFileSizeInBytes = totalFileSizeInKilobytes * 1024;

        return totalFileSizeInBytes;
    }


    public void CreateStorage(Storage storage)
    {
        Create(storage);
    }

    public void DeleteStorage(Storage storage)
    {
        Delete(storage);
    }
}
