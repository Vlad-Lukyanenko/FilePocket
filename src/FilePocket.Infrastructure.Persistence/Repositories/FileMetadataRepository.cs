using FilePocket.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FilePocket.Application.Interfaces.Repositories;
using FilePocket.Domain;

namespace FilePocket.Infrastructure.Persistence.Repositories;

public class FileMetadataRepository : RepositoryBase<FileMetadata>, IFileMetadataRepository
{
    public FileMetadataRepository(FilePocketDbContext context)
        : base(context)
    {
    }

    public async Task<List<FileMetadata>> GetAllAsync(Guid pocketId, bool trackChanges)
    {
        return await FindByCondition(f => f.PocketId.Equals(pocketId) && f.FolderId == null && !f.IsDeleted && f.FileType != Domain.FileTypes.Note, trackChanges).ToListAsync();
    }

    public async Task<List<FileMetadata>> GetRecentFilesAsync(Guid userId, int numberOfFiles)
    {
        return await FindByCondition(f => f.UserId.Equals(userId) && !f.IsDeleted && f.FileType != Domain.FileTypes.Note, false).OrderByDescending(c => c.CreatedAt).Take(numberOfFiles).ToListAsync();
    }

    public async Task<List<FileMetadata>> GetAllAsync(Guid userId, Guid pocketId, Guid? folderId, bool isSoftDeleted, bool trackChanges)
    {
        return await FindByCondition(f => f.UserId.Equals(userId) 
                                            && f.PocketId.Equals(pocketId) 
                                            && f.FolderId.Equals(folderId) 
                                            && f.IsDeleted == isSoftDeleted
                                            && f.FileType != Domain.FileTypes.Note, trackChanges).ToListAsync();
    }

    public async Task<List<FileMetadata>> GetAllNotesAsync(Guid userId, Guid? folderId, bool isSoftDeleted, bool trackChanges = false)
    {
        return await FindByCondition(f => f.UserId.Equals(userId)
                                    && f.FolderId.Equals(folderId)
                                    && f.IsDeleted == isSoftDeleted
                                    && f.FileType == Domain.FileTypes.Note, trackChanges).ToListAsync();
    }
    public async Task<List<FileMetadata>> GetAllWithSoftDeletedAsync(Guid userId, Guid pocketId, bool trackChanges)
    {
        return await FindByCondition(f => f.UserId.Equals(userId)
                                            && f.PocketId.Equals(pocketId), trackChanges).ToListAsync();
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

    public async  Task<List<FileMetadata>> GetFileMetadataByPartialNameAsync(Guid userId, string partialName, bool trackChanges = false)
    {
        return (await FindByCondition(f => f.UserId.Equals(userId) && f.OriginalName.ToLower().Contains(partialName), trackChanges)
            .OrderBy(f=>f.FileType)
            .ToListAsync());
    }
    private static bool SelectByFileType(FileMetadata fileMetadata, FileTypes? fileType)
    {
        if (fileType != null)
        {
            return fileMetadata.FileType == fileType;
        }
        return fileMetadata.FileType != Domain.FileTypes.Note;
    }
}

