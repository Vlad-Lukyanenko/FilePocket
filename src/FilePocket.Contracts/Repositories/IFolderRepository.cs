using FilePocket.Domain.Entities;

namespace FilePocket.Contracts.Repositories
{
    public interface IFolderRepository
    {
        void Create(Folder folder);
        Task Delete(Guid folderId);
        void DeleteByPocketId(Guid pocketId);
        Task<Folder?> GetAsync(Guid folderId);
        Task<List<Folder>> GetAllAsync(Guid userId, Guid? pocketId, Guid? parentFolderId);
        Task<bool> ExistsAsync(string folderName, Guid? pocketId, Guid? parentFolderId);

    }
}
