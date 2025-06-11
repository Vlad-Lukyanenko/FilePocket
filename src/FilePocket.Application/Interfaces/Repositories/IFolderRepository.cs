using FilePocket.Domain.Entities;
using FilePocket.Domain.Enums;

namespace FilePocket.Application.Interfaces.Repositories;

public interface IFolderRepository
{
    IEnumerable<Folder> GetChildFolders(Guid parentFolderId, bool trackChanges);
    IEnumerable<Folder> GetAll(Guid userId, bool isSoftDeleted, bool trackChanges);
    void Create(Folder folder);
    Task Delete(Guid folderId);
    void DeleteByPocketId(Guid pocketId);
    Task<Folder?> GetAsync(Guid folderId);
    Task<bool> ExistsAsync(string folderName, Guid? pocketId, Guid? parentFolderId, FolderType folderType);
    Task<List<Folder>> GetAllAsync(Guid userId, Guid? pocketId, Guid? parentFolderId, List<FolderType> folderTypes, bool isSoftDeleted);
    Task<List<Folder>> GetFoldersByPartialNameAsync(Guid userId, string partialName, bool trackChanges = false);
    Task<List<Folder>> GetAllSoftDeletedAsync(Guid userId, bool trackChanges);
    void DeleteFolders(IEnumerable<Folder> folders);
}
