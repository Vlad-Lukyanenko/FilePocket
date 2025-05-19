using FilePocket.Domain.Enums;
using FilePocket.Domain.Models;

namespace FilePocket.Application.Interfaces.Services;

public interface IFolderService
{
    Task<FolderModel> CreateAsync(FolderModel folder);
    Task<FolderModel?> GetAsync(Guid folderId);
    Task<List<FolderModel>> GetAllAsync(Guid userId, Guid? pocketId, Guid? parentFolderId, List<FolderType> folderTypes, bool isSoftDeleted);
    Task DeleteAsync(Guid folderId);
    Task DeleteByPocketIdAsync(Guid pocketId);
    Task MoveToTrashAsync(Guid folderId);
    Task RestoreFromTrashAsync(Guid folderId);
    void DeleteAllFolders(Guid userId);
}
