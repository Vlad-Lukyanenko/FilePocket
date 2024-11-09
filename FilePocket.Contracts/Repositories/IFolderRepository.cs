using FilePocket.Domain.Entities;

namespace FilePocket.Contracts.Repositories
{
    public interface IFolderRepository
    {
        void Create(Folder folder);
        void Delete(Guid folderId);
        Task<List<Folder>> GetAllAsync(Guid pocketId, Guid? parentFolderId);
    }
}
