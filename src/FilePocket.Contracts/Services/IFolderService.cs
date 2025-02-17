using FilePocket.Domain.Models;

namespace FilePocket.Contracts.Services
{
    public interface IFolderService
    {
        Task<FolderModel> CreateAsync(FolderModel folder);
        Task<FolderModel?> GetAsync(Guid folderId);
        Task<List<FolderModel>> GetAllAsync(Guid userId, Guid? pocketId, Guid? parentFolderId);
        Task DeleteAsync(Guid folderId);
        Task DeleteByPocketIdAsync(Guid pocketId);
        Task MoveToTrash(Guid userId, Guid folderId);
    }
}
