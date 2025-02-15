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

    public async Task<List<FileMetadata>> GetAllAsync(Guid pocketId, bool trackChanges)
    {
        return await FindByCondition(e => e.PocketId.Equals(pocketId) && e.FolderId == null, trackChanges).ToListAsync();
    }

    public async Task<List<FileMetadata>> GetRecentFilesAsync(Guid userId, int numberOfFiles)
    {
        return await FindByCondition(e => e.UserId.Equals(userId), false).OrderByDescending(c => c.DateCreated).Take(numberOfFiles).ToListAsync();
    }

    public async Task<List<FileMetadata>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool trackChanges)
    {
        return await FindByCondition(e => e.UserId.Equals(userId) && e.PocketId.Equals(pocketId) && e.FolderId.Equals(folderId), trackChanges).ToListAsync();
    }

    public async Task<FileMetadata> GetByIdAsync(Guid? pocketId, Guid fileId, bool trackChanges)
    {
        return (await FindByCondition(e => e.PocketId.Equals(pocketId) && e.Id.Equals(fileId), trackChanges).SingleOrDefaultAsync())!;
    }
    
    public async Task<FileMetadata> GetByIdAsync(Guid userId, Guid fileId, bool trackChanges = false)
    {
        return (await FindByCondition(e => e.UserId.Equals(userId) && e.Id.Equals(fileId), trackChanges).SingleOrDefaultAsync())!;
    }

    public void CreateFileMetadata(FileMetadata fileMetadataId)
    {
        Create(fileMetadataId);
    }

    public void UpdateFileMetadata(FileMetadata fileMetadataId)
    {
        Update(fileMetadataId);
    }

    public void DeleteFileMetadata(FileMetadata fileMetadata)
    {
        Delete(fileMetadata);
    }
}
