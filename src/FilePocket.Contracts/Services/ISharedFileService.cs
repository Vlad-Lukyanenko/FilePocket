using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.Contracts.Services
{
    public interface ISharedFileService
    {
        Task CreateAsync(Guid userId, SharedFileModel sharedFile);

        Task<SharedFileModel?> GetByIdAsync(Guid sharedFileId);

        Task<byte[]?> DownloadFileAsync(Guid sharedFileId);

        Task<List<SharedFileView>> GetAllAsync(Guid userId, bool trackChanges);

        Task Delete(Guid sharedFileId);

        Task<List<SharedFileView>> GetLatestAsync(Guid userId, int number);
    }
}
