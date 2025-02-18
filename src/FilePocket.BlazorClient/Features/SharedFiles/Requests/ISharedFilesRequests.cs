using FilePocket.BlazorClient.Features.SharedFiles.Models;

namespace FilePocket.BlazorClient.Features.SharedFiles.Requests
{
    public interface ISharedFilesRequests
    {
        string BaseAddress { get; }

        Task<Guid> CreateAsync(SharedFileModel sharedFile);

        Task<SharedFileModel?> GetByIdAsync(Guid sharedFileId);

        Task<List<SharedFileView>> GetAllAsync();

        Task<List<SharedFileView>> GetLatestAsync();

        Task<byte[]?> DownloadAsync(Guid sharedFileId);

        Task<bool> DeleteAsync(Guid sharedFileId);
    }
}
