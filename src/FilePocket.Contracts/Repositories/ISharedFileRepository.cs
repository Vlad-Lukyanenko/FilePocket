using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;

namespace FilePocket.Contracts.Repositories
{
    public interface ISharedFileRepository
    {
        void Create(SharedFile sharedFile);

        Task<SharedFile?> GetByIdAsync(Guid sharedFileId);

        Task<SharedFileModel?> GetAggregatedDataByIdAsync(Guid sharedFileId);

        Task<List<SharedFileView>> GetAllAsync(Guid userId, bool trackChanges);

        Task<List<SharedFileView>> GetLatestAsync(Guid userId, int number, bool trackChanges);

        void Delete(SharedFile sharedFile);

        Task<DownloadFileModel?> GetFileBodyAsync(Guid sharedFileId);
    }
}
