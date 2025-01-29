using FilePocket.Client.Services.Files.Models;

namespace FilePocket.Client.Services.Files.Requests
{
    public interface IFileRequests
    {
        Task<List<FileInfoModel>> GetFilesAsync(Guid? pocketId, Guid? folderId);
        Task<List<FileInfoModel>> GetRecentFilesAsync();
        Task<FileModel> GetFileAsync(Guid fileId);
        Task<FileModel> GetImageThumbnailAsync(Guid imageId, int size);
        Task<FileModel> GetFileInfoAsync(Guid fileId);
        Task<FileModel> UploadFileAsync(MultipartFormDataContent content);
        Task DeleteFile(Guid fileId);
    }
}
