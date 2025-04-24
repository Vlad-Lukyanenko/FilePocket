using FilePocket.BlazorClient.Features.Folders.Models;
using FilePocket.BlazorClient.Shared.Enums;

namespace FilePocket.BlazorClient.Services.Folders.Requests;

public interface IFolderRequests
{
    Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, Guid parentFolderId, List<FolderType> folderTypes, bool isSoftDeleted);
    Task<FolderModel> GetAsync(Guid pocketId, Guid folderId);
    Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, List<FolderType> folderTypes, bool isSoftDeleted);
    Task<bool> CreateAsync(FolderModel folder);
    Task<bool> DeleteAsync(Guid folderId);
    Task<bool> SoftDeleteAsync(Guid folderId);
    Task<bool> RestoreAsync(Guid folderId);
    Task<bool> UpdateAsync(FolderModel folder);
}
