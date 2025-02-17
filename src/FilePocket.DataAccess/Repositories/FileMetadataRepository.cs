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
        return await FindByCondition(f => f.PocketId.Equals(pocketId) && f.FolderId == null && !f.IsDeleted, trackChanges).ToListAsync();
    }

    public async Task<List<FileMetadata>> GetRecentFilesAsync(Guid userId, int numberOfFiles)
    {
        return await FindByCondition(f => f.UserId.Equals(userId) && !f.IsDeleted, false).OrderByDescending(c => c.CreatedAt).Take(numberOfFiles).ToListAsync();
    }

    public async Task<List<FileMetadata>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool trackChanges)
    {
        return await FindByCondition(f => f.UserId.Equals(userId) && f.PocketId.Equals(pocketId) && f.FolderId.Equals(folderId) && !f.IsDeleted, trackChanges).ToListAsync();
    }
    
    public async Task<FileMetadata> GetByIdAsync(Guid userId, Guid fileId, bool trackChanges = false)
    {
        return (await FindByCondition(f => f.UserId.Equals(userId) && f.Id.Equals(fileId), trackChanges).SingleOrDefaultAsync())!;
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
