using FilePocket.Domain.Models;

namespace FilePocket.Contracts.Services
{
    public interface IFolderService
    {
        Task CreateAsync(FolderModel folder);
        Task<List<FolderModel>>  GetAllAsync( Guid pocketId, Guid? parentFolderId);
        Task DeleteAsync(Guid folderId);
    }
}
