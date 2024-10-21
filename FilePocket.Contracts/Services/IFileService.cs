using FilePocket.Domain.Entities;
using FilePocket.Domain.Models;
using MagpieChat.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace FilePocket.Contracts.Services;

public interface IFileService
{
    IUploadService UploadService { get; }

    Session CreateSession(Guid userId, Guid storageId, CreateSessionParams sessionParams);

    Task<IEnumerable<FileResponseModel>> GetAllFilesByStorageIdAsync(Guid storageId);

    Task<IEnumerable<FileResponseModel>> GetFilteredFilesAsync(FilesFilterOptionsModel filterOptionsModel);

    Task<int> GetFilteredFilesCountAsync(FilesFilterOptionsModel filterOptionsModel);

    Task<FileResponseModel> GetFileByIdAsync(Guid storageId, Guid id);

    Task<FileResponseModel> GetImageThumbnailAsync(Guid storageId, Guid id, int maxSize);

    Task<List<FileResponseModel>> GetImageThumbnailsAsync(List<UserIconInfoRequest> request, Guid storageId, int maxSize);

    Task<FileUploadSummary> UploadFileAsync(IFormFile file, Guid userId, Guid storageId);

    Task UploadFileChunkAsync(IFormFile file, string userId, string storageId, string sessionId, int chunkNumber);

    Task DeleteFileAsync(Guid storageId, Guid id);

    Task<bool> CheckIfFileExists(string fileName, Guid storageId);


}
