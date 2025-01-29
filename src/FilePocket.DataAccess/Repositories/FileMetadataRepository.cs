using FilePocket.Contracts.Repositories;
using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FilePocket.DataAccess.Repositories;

public class FileMetadataRepository : RepositoryBase<FileMetadata>, IFileMetadataRepository
{
    public FileMetadataRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public async Task<List<FileMetadata>> GetAllAsync(Guid? pocketId, bool trackChanges)
    {
        return await FindByCondition(e => e.PocketId.Equals(pocketId) && e.FolderId == null && !e.IsDeleted, trackChanges).ToListAsync();
    }

    public async Task<List<FileMetadata>> GetRecentFilesAsync(Guid userId, int numberOfFiles)
    {
        return await FindByCondition(e => e.UserId.Equals(userId) && !e.IsDeleted, false).OrderByDescending(c => c.DateCreated).Take(numberOfFiles).ToListAsync();
    }

    public async Task<List<FileMetadata>> GetAllAsync(Guid userId, Guid? pocketId, Guid? folderId, bool trackChanges)
    {
        return await FindByCondition(e => e.UserId.Equals(userId) && e.PocketId.Equals(pocketId) && e.FolderId.Equals(folderId) && !e.IsDeleted, trackChanges).ToListAsync();
    }

    public async Task<FileMetadata> GetByIdAsync(Guid? pocketId, Guid fileId, bool trackChanges)
    {
        return (await FindByCondition(e => e.PocketId.Equals(pocketId) && e.Id.Equals(fileId) && !e.IsDeleted, trackChanges).SingleOrDefaultAsync())!;
    }
    
    public async Task<FileMetadata> GetByIdAsync(Guid fileId, bool trackChanges = false)
    {
        return (await FindByCondition(e => e.Id.Equals(fileId) && !e.IsDeleted, trackChanges).SingleOrDefaultAsync())!;
    }

    public void CreateFileMetadata(FileMetadata fileMetadataId)
    {
        Create(fileMetadataId);
    }

    public void UpdateFileMetadata(FileMetadata fileMetadataId)
    {
        Update(fileMetadataId);
    }
}
