using FilePocket.Client.Services.Files.Models;

namespace FilePocket.Client.Services.Files.Requests
{
    public interface IFileRequests
    {
        Task<List<FileInfoModel>> GetFilesAsync(Guid pocketId);
        Task<List<FileInfoModel>> GetFilesAsync(Guid pocketId, Guid folderId);
        Task<FileModel> GetFileAsync(Guid pocketId, Guid fileId);
        Task<FileModel> GetImageThumbnailAsync(Guid pocketId, Guid imageId, int size);
        Task<FileModel> GetFileInfoAsync(Guid pocketId, Guid fileId);
        Task<FileModel> UploadFileAsync(MultipartFormDataContent content, Guid pocketId);
        Task DeleteFile(Guid pocketId, Guid fileId);
    }
}
