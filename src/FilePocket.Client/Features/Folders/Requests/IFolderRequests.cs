using FilePocket.Client.Features.Folders.Models;

namespace FilePocket.Client.Services.Folders.Requests
{
    public interface IFolderRequests
    {
        Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId, Guid parentFolderId);
        Task<FolderModel> GetAsync(Guid folderId);
        Task<IEnumerable<FolderModel>> GetAllAsync(Guid? pocketId);
        Task<bool> CreateAsync(FolderModel folder);
        Task<bool> DeleteAsync(Guid folderId);
        Task<bool> UpdateAsync(FolderModel folder);
    }
}
