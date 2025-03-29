using FilePocket.BlazorClient.Features.Folders.Models;
using FilePocket.BlazorClient.Shared.Enums;

namespace FilePocket.BlazorClient.Services.Folders.Requests
{
    public interface IFolderRequests
    {
        Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, Guid parentFolderId, FolderType folderType);
        Task<FolderModel> GetAsync(Guid pocketId, Guid folderId);
        Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, FolderType folderType);
        Task<bool> CreateAsync(FolderModel folder);
        Task<bool> DeleteAsync(Guid folderId);
        Task<bool> UpdateAsync(FolderModel folder);
    }
}
