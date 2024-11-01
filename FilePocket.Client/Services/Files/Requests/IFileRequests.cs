using FilePocket.Client.Services.Files.Models;

namespace FilePocket.Client.Services.Files.Requests
{
    public interface IFileRequests
    {
        Task<List<FileInfoModel>> GetFilesAsync(Guid pocketId);
        Task<FileModel> GetFileAsync(Guid pocketId, Guid fileId);
    }
}
