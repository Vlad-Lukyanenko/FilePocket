using FilePocket.BlazorClient.Features.Files.Models;
using FilePocket.BlazorClient.Services.Files.Models;

namespace FilePocket.BlazorClient.Services.Files.Requests;

public interface IFileRequests
{
    Task<List<FileInfoModel>> GetFilesAsync(Guid? pocketId, Guid? folderId, bool isSoftDeleted);
    Task<List<FileInfoModel>> GetAllFilesWithSoftDeletedAsync(Guid? pocketId);
    Task<List<FileInfoModel>> GetRecentFilesAsync();
    Task<FileModel> GetFileAsync(Guid fileId);
    Task<FileModel> GetImageThumbnailAsync(Guid imageId, int size);
    Task<FileModel> GetFileInfoAsync(Guid fileId);
    Task<FileModel> UploadFileAsync(MultipartFormDataContent content);
    Task<bool> DeleteFile(Guid fileId);
    Task<bool> UpdateFileAsync(UpdateFileInfoModel file);
    Task <List<FileSearchResponseModel>> GetFilesByPartialNameAsync(string partialNameToSearch);
}
