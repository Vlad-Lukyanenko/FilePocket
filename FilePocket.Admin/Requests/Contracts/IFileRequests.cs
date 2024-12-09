using FilePocket.Admin.Models;
using FilePocket.Admin.Models.Files;
using FilePocket.Domain.Models;

namespace FilePocket.Admin.Requests.Contracts
{
    public interface IFileRequests
    {
        Task<IEnumerable<FileModel>> GetAllAsync(Guid storageId);
        Task<FilteredFilesModel> GetFilteredAsync(Models.Files.FilesFilterOptionsModel filterOptions);
        Task<FileDownloadModel> GetById(Guid id, Guid storageId);
        Task<bool> PutAsync(FileModel file, Guid storageId);
        Task<bool> DeleteAsync(Guid id, Guid storageId);
        Task<FileModel> PostAsync(MultipartFormDataContent content, Guid storageId);
        Task<bool> PostAsync(MultipartFormDataContent content, SessionModel session, int chunkNumber);
        Task<bool> CheckIfFileExists(string fileName, Guid storageId);
        Task<SessionModel> CreateSession(Guid userId, Guid storageId, CreateSessionParams createParams);
        Task<ImageResponseModel> GetThumbnailAsync(Guid storageId, Guid imageId, int size);
    }
}
