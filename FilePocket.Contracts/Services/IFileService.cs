using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace FilePocket.Contracts.Services;

public interface IFileService
{
    IUploadService UploadService { get; }

    Session CreateSession(Guid userId, Guid storageId, CreateSessionParams sessionParams);

    Task<IEnumerable<FileResponseModel>> GetAllFilesFromPocketAsync(Guid pocketId);

    Task<IEnumerable<FileResponseModel>> GetAllFilesFromPocketAsync(Guid pocketId, Guid folderId);

    Task<IEnumerable<FileResponseModel>> GetFilteredFilesAsync(FilesFilterOptionsModel filterOptionsModel);

    Task<int> GetFilteredFilesCountAsync(FilesFilterOptionsModel filterOptionsModel);

    Task<FileResponseModel> GetFileByIdAsync(Guid pocketId, Guid fileId);

    Task<FileResponseModel> GetFileInfoByIdAsync(Guid pocketId, Guid fileId);

    Task<FileResponseModel> GetThumbnailAsync(Guid pocketId, Guid fileId, int maxSize);

    Task<List<FileResponseModel>> GetThumbnailsAsync(Guid storageId, Guid[] fileIds, int maxSize);

    Task<FileUploadSummary> UploadFileAsync(IFormFile file, Guid userId, Guid storageId, Guid? folderId);

    Task UploadFileChunkAsync(IFormFile file, string userId, string storageId, string sessionId, int chunkNumber);

    Task DeleteFileAsync(Guid storageId, Guid id);

    Task<bool> CheckIfFileExists(string fileName, Guid storageId);


}
