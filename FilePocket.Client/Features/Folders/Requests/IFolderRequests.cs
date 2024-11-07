using FilePocket.Client.Services.Pockets.Models;

namespace FilePocket.Client.Services.Folders.Requests
{
    public interface IFolderRequests
    {
        Task<IEnumerable<PocketModel>> GetAllAsync(Guid userId);
        Task<bool> CreateAsync(CreatePocketModel pocket);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateAsync(PocketModel pocket);
    }
}
