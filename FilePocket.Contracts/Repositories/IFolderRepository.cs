using FilePocket.Domain.Entities;

namespace FilePocket.Contracts.Repositories
{
    public interface IFolderRepository
    {
        void Create(Folder folder);
        void Delete(Guid folderId);
        void DeleteByPocketId(Guid pocketId);
        Task<Folder?> GetAsync(Guid folderId);
        Task<List<Folder>> GetAllAsync(Guid pocketId, Guid? parentFolderId);
    }
}
