using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace FilePocket.Contracts.Services;

public interface IFileService
{
    IUploadService UploadService { get; }

    Task<IEnumerable<FileResponseModel>> GetAllFilesAsync(Guid userId, Guid? pocketId, Guid? folderId);

    Task<FileResponseModel> GetFileByIdAsync(Guid userId, Guid fileId);

    Task<FileResponseModel> GetFileInfoByIdAsync(Guid userId, Guid fileId);

    Task<List<FileResponseModel>> GetRecentFiles(Guid userId, int number);

    Task<FileResponseModel> GetThumbnailAsync(Guid userId, Guid fileId, int maxSize);

    Task<List<FileResponseModel>> GetThumbnailsAsync(Guid userId, Guid[] fileIds, int maxSize);

    Task<FileResponseModel> UploadFileAsync(IFormFile file, Guid userId, Guid? pocketId, Guid? folderId);

    Task DeleteFileAsync(Guid userId, Guid id);
}
