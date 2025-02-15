using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace FilePocket.Contracts.Services;

// Handles read concerns
public interface IFileProvider
{
    Task<IEnumerable<FileResponseModel>> GetAllFilesAsync(
        Guid userId,
        Guid pocketId,
        Guid? folderId);

    Task<FileResponseModel> GetFileByIdAsync(
        Guid userId,
        Guid fileId);

    Task<FileResponseModel> GetFileInfoByIdAsync(
        Guid userId,
        Guid fileId);

    Task<List<FileResponseModel>> GetRecentFiles(
        Guid userId,
        int number);

    Task<FileResponseModel> GetThumbnailAsync(
        Guid userId,
        Guid fileId,
        int maxSize);

    Task<List<FileResponseModel>> GetThumbnailsAsync(
        Guid userId,
        Guid[] fileIds,
        int maxSize);
}

// Handles write concerns (and read concerns for backward compatibility right now)
public interface IFileService : IFileProvider
{
    Task<FileResponseModel?> UploadFileAsync(
        Guid userId,
        IFormFile file,
        Guid pocketId,
        Guid? folderId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveFileAsync(
        Guid userId,
        Guid fileId,
        CancellationToken cancellationToken = default);
}