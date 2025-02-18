using FilePocket.BlazorClient.Services.Pockets.Models;

namespace FilePocket.BlazorClient.Services.Pockets.Requests
{
    public interface IPocketRequests
    {
        Task<IEnumerable<PocketModel>> GetAllCustomAsync();
        Task<PocketModel> GetDefaultAsync();
        Task<PocketModel> GetInfoAsync(Guid id);
        Task<bool> CreateAsync(CreatePocketModel pocket);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> UpdateAsync(PocketModel pocket);
        Task<bool> CheckPocketCapacityAsync(Guid pocket, long fileSize);
    }
}
